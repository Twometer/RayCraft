use std::io::{Error, Read, Write};
use std::net::TcpStream;

use super::{calc_varint_size, read_var_int, ReadBuffer, WriteBuffer};

#[derive(Debug)]
pub enum State {
    Login = 2,
    Play,
}

pub struct Client {
    socket: TcpStream,
    compression_threshold: usize,
    state: State,
}

impl Client {
    pub fn connect(addr: &str) -> Result<Client, Error> {
        match TcpStream::connect(addr) {
            Ok(stream) => {
                println!("Connected to minecraft server");
                return Ok(Client {
                    socket: stream,
                    compression_threshold: 0,
                    state: State::Login,
                });
            }
            Err(e) => {
                println!("Failed to connect: {}", e);
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

    pub fn send_packet(&mut self, id: u32, payload: &WriteBuffer) {
        let mut packet = WriteBuffer::new();

        if self.compression_threshold > 0 {
            // FIXME: The Minecraft protocol requires packets to be of a different format when compression is enabled.
            //        However, packets below a certain threshold are not actually compressed. This code does not implement
            //        the actual compression, which is why outbound packets which are larger than the threshold (typically
            //        256 bytes)  will probably cause a disconnect. However, 99% of packets are <256 b
            let packet_len = calc_varint_size(id) + calc_varint_size(0) + payload.len();
            packet.write_varint(packet_len as u32);
            packet.write_varint(0);
        } else {
            let packet_len = calc_varint_size(id) + payload.len();
            packet.write_varint(packet_len as u32);
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
        payload.write_varint(next_state as u32);
        self.send_packet(0x00, &payload);
    }

    pub fn send_login(&mut self, username: &str) {
        let mut payload = WriteBuffer::new();
        payload.write_string(username);
        self.send_packet(0x00, &payload);
    }

    pub fn send_keep_alive(&mut self, timestamp: u32) {
        let mut payload = WriteBuffer::new();
        payload.write_varint(timestamp);
        self.send_packet(0x00, &payload);
    }

    fn handle_login_packet(&mut self, packet_id: u32, mut packet_buf: ReadBuffer) {
        match packet_id {
            0x02 => {
                println!("login completed");
                self.state = State::Play;
            }
            0x03 => {
                self.compression_threshold = packet_buf.read_var_int() as usize;
                println!("set compression threshold: {}", self.compression_threshold);
            }
            _ => {}
        }
    }

    fn handle_play_packet(&mut self, packet_id: u32, mut packet_buf: ReadBuffer) {
        println!("Received packet id #{}", packet_id,);
        match packet_id {
            0x00 => {
                // Keep Alive
                let timestamp = packet_buf.read_var_int();
                println!("keepalive {}", timestamp);
                self.send_keep_alive(timestamp);
            }
            0x01 => {
                // Join Game
                let entity_id = packet_buf.read_var_int();
                let game_mode = packet_buf.read_byte();
                println!(
                    "logged in with entity id {} and gamemode {}",
                    entity_id, game_mode
                );
            }
            _ => {}
        }
    }
}
