use std::io::{Error, Write};
use std::net::TcpStream;

use super::Buffer;

pub struct Client {
    socket: TcpStream,
}

impl Client {
    pub fn connect(addr: &str) -> Result<Client, Error> {
        match TcpStream::connect(addr) {
            Ok(stream) => {
                println!("Connected to minecraft server");
                return Ok(Client { socket: stream });
            }
            Err(e) => {
                println!("Failed to connect: {}", e);
                return Err(e);
            }
        }
    }

    pub fn login(&mut self, username: &str) {
        self.send_handshake("dummy", 0, 2);
        self.send_login(username);
    }

    pub fn send_packet(&mut self, id: u32, payload: &Buffer) {
        let mut packet = Buffer::new();
        let packet_len = calc_varint_size(id) + payload.len();
        packet.write_varint(packet_len as u32);
        packet.write_varint(id);
        packet.write_buf(payload);
        self.socket
            .write(packet.data())
            .expect("failed to send packet");
    }

    pub fn send_handshake(&mut self, hostname: &str, port: u16, next_state: u32) {
        let mut payload = Buffer::new();
        payload.write_varint(47); // Protocol version for 1.8.x
        payload.write_string(hostname);
        payload.write_u16(port);
        payload.write_varint(next_state);
        self.send_packet(0x00, &payload);
    }

    pub fn send_login(&mut self, username: &str) {
        let mut payload = Buffer::new();
        payload.write_string(username);
        self.send_packet(0x00, &payload);
    }
}

fn calc_varint_size(mut value: u32) -> usize {
    let mut size: usize = 0;
    loop {
        value >>= 7;
        size += 1;
        if value == 0 {
            break;
        }
    }
    return size;
}