use std::io::{Error, Read, Write};
use std::net::TcpStream;

use super::{block_pos_to_idx, calc_varint_size, read_var_int, ReadBuffer, WriteBuffer};

#[derive(Debug)]
pub enum State {
    Login = 2,
    Play,
}

pub struct Chunk {
    x: i32,
    z: i32,
    continuous: bool,
    bitmask: u16,
    block_data: Vec<u8>,
}

#[derive(Default)]
pub struct World {
    pub time: i64,
    pub age: i64,
}

#[derive(Default)]
pub struct Player {
    pub entity_id: i32,
    pub game_mode: u8,
    pub health: f32,
    pub hunger: i32,
    pub pos_x: f32,
    pub pos_y: f32,
    pub pos_z: f32,
    pub rot_x: f32,
    pub rot_y: f32,
}

pub struct Client {
    pub player: Player,
    pub world: World,
    socket: TcpStream,
    compression_threshold: usize,
    state: State,
}

impl Client {
    pub fn connect(addr: &str) -> Result<Client, Error> {
        match TcpStream::connect(addr) {
            Ok(stream) => {
                return Ok(Client {
                    socket: stream,
                    compression_threshold: 0,
                    state: State::Login,
                    player: Player::default(),
                    world: World::default(),
                });
            }
            Err(e) => {
                return Err(e);
            }
        }
    }

    pub fn login(&mut self, username: &str) {
        self.send_handshake("hostname", 0, State::Login);
        self.send_login(username);
    }

    pub fn receive(&mut self) {
        let packet_len = read_var_int(&mut self.socket);
        if packet_len <= 0 {
            return;
        }

        let mut data = vec![0u8; packet_len as usize];
        self.socket
            .read_exact(&mut data)
            .expect("failed to read packet contents");

        let mut buf = ReadBuffer::new(data);
        if self.compression_threshold > 0 {
            let size_uncompressed = buf.read_var_int();
            if size_uncompressed > 0 {
                buf.decompress();
            }
        }

        let packet_id = buf.read_var_int();
        match self.state {
            State::Login => self.handle_login_packet(packet_id, buf),
            State::Play => self.handle_play_packet(packet_id, buf),
        }
    }

    pub fn send_packet(&mut self, id: i32, payload: &WriteBuffer) {
        let mut packet = WriteBuffer::new();

        if self.compression_threshold > 0 {
            // FIXME: The Minecraft protocol requires packets to be of a different format when compression is enabled.
            //        However, packets below a certain threshold are not actually compressed. This code does not implement
            //        the actual compression, which is why outbound packets which are larger than the threshold (typically
            //        256 bytes)  will probably cause a disconnect. However, 99% of packets are <256 b
            let packet_len = calc_varint_size(id) + calc_varint_size(0) + payload.len();
            packet.write_varint(packet_len as i32);
            packet.write_varint(0);
        } else {
            let packet_len = calc_varint_size(id) + payload.len();
            packet.write_varint(packet_len as i32);
        }

        packet.write_varint(id);
        packet.write_buf(payload);

        self.socket
            .write(packet.data())
            .expect("failed to send packet");
    }

    pub fn send_handshake(&mut self, hostname: &str, port: u16, next_state: State) {
        let mut payload = WriteBuffer::new();
        payload.write_varint(47); // Protocol version for 1.8.x
        payload.write_string(hostname);
        payload.write_u16(port);
        payload.write_varint(next_state as i32);
        self.send_packet(0x00, &payload);
    }

    pub fn send_login(&mut self, username: &str) {
        let mut payload = WriteBuffer::new();
        payload.write_string(username);
        self.send_packet(0x00, &payload);
    }

    pub fn send_keep_alive(&mut self, timestamp: i32) {
        let mut payload = WriteBuffer::new();
        payload.write_varint(timestamp);
        self.send_packet(0x00, &payload);
    }

    fn handle_login_packet(&mut self, packet_id: i32, mut packet_buf: ReadBuffer) {
        match packet_id {
            0x02 => {
                self.state = State::Play;
                println!("Login completed");
            }
            0x03 => {
                self.compression_threshold = packet_buf.read_var_int() as usize;
                println!("Set compression threshold: {}", self.compression_threshold);
            }
            _ => {}
        }
    }

    fn handle_play_packet(&mut self, packet_id: i32, mut packet_buf: ReadBuffer) {
        match packet_id {
            0x00 => {
                // Keep Alive
                let timestamp = packet_buf.read_var_int();
                self.send_keep_alive(timestamp);
            }
            0x01 => {
                // Join Game
                self.player.entity_id = packet_buf.read_i32();
                self.player.game_mode = packet_buf.read_u8();
                println!(
                    "Logged in with entity id {} and gamemode {}",
                    self.player.entity_id, self.player.game_mode
                );
            }
            0x02 => {
                // Chat
                let msg = packet_buf.read_string();
                println!("Received chat message: {}", msg);
            }
            0x03 => {
                // Time
                self.world.age = packet_buf.read_i64();
                self.world.time = packet_buf.read_i64();
            }
            0x06 => {
                // Health
                self.player.health = packet_buf.read_f32();
                self.player.hunger = packet_buf.read_var_int();
                println!(
                    "Updated health={},hunger={}",
                    self.player.health, self.player.hunger
                );
            }
            0x08 => {
                // PosLook
                self.player.pos_x = packet_buf.read_f64() as f32;
                self.player.pos_y = packet_buf.read_f64() as f32;
                self.player.pos_z = packet_buf.read_f64() as f32;

                self.player.rot_x = packet_buf.read_f32();
                self.player.rot_y = packet_buf.read_f32();
                println!(
                    "Teleported player to x={}, y={}, z={}",
                    self.player.pos_x, self.player.pos_y, self.player.pos_z
                );
            }
            0x26 => {
                // Chunk bulk
                let has_skylight = packet_buf.read_bool();
                let num_chunks = packet_buf.read_var_int();

                let mut chunks = Vec::<Chunk>::new();
                for _ in 1..num_chunks {
                    chunks.push(Chunk {
                        x: packet_buf.read_i32(),
                        z: packet_buf.read_i32(),
                        continuous: true,
                        bitmask: packet_buf.read_u16(),
                        block_data: vec![0; 16 * 16 * 256],
                    });
                }

                for mut chunk in chunks {
                    let sections_read = chunk.read_from(&mut packet_buf);
                    packet_buf.skip(compute_unused_chunk_data_size(
                        sections_read,
                        has_skylight,
                        &chunk,
                    ) as u64);
                    self.world.add_chunk(chunk);
                }

                println!("Received {} chunks", num_chunks);
            }
            _ => {}
        }
    }
}

impl Chunk {
    pub fn read_from(&mut self, packet_buf: &mut ReadBuffer) -> i32 {
        let mut sections_read: i32 = 0;

        // Iterate possible sections
        for idx in 0..15 {
            // Check if section is present
            let section_mask = 1 << idx;
            if self.bitmask & section_mask != 0 {
                self.read_section_from(idx, packet_buf);
                sections_read += 1;
            }
        }

        return sections_read;
    }

    fn read_section_from(&mut self, idx: u32, packet_buf: &mut ReadBuffer) {
        let base_dst_idx = block_pos_to_idx(0, idx * 16, 0);

        for idx in 0..(16 * 16 * 16) - 1 {
            let block_data = packet_buf.read_u16_le();

            // Currently, we only use the block_id
            // let block_meta = block_data & 15;
            let block_id = block_data >> 4;

            let dst_idx = base_dst_idx + idx;
            self.block_data[dst_idx as usize] = block_id as u8;
        }
    }
}

impl World {
    pub fn add_chunk(&mut self, chunk: Chunk) {
        // TODO
    }
}

fn compute_unused_chunk_data_size(sections_read: i32, has_skylight: bool, chunk: &Chunk) -> i32 {
    let light_size = sections_read * 16 * 16 * 8;
    let sky_light_size = if has_skylight { light_size } else { 0 };
    let biomes_size = if chunk.continuous { 256 } else { 0 };
    return light_size + sky_light_size + biomes_size;
}
