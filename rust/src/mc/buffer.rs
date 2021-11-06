use std::io::{Cursor, Read};

use super::read_var_int;

pub struct WriteBuffer {
    buf: Vec<u8>,
}

pub struct ReadBuffer {
    buf: Cursor<Vec<u8>>,
}

impl WriteBuffer {
    pub fn new() -> WriteBuffer {
        WriteBuffer { buf: Vec::new() }
    }

    pub fn write_varint(&mut self, mut value: u32) {
        loop {
            let mut cur_byte = (value & 0x7f) as u8;
            value >>= 7;
            if value != 0 {
                cur_byte |= 0x80;
            }
            self.buf.push(cur_byte);
            if value == 0 {
                break;
            }
        }
    }

    pub fn write_u16(&mut self, value: u16) {
        let bytes = value.to_be_bytes();
        self.write_bytes(&bytes);
    }

    pub fn write_string(&mut self, value: &str) {
        self.write_varint(value.len() as u32);
        let bytes = value.as_bytes();
        self.write_bytes(&bytes);
    }

    pub fn write_buf(&mut self, other: &WriteBuffer) {
        self.buf.extend(&other.buf);
    }

    fn write_bytes(&mut self, bytes: &[u8]) {
        self.buf.extend_from_slice(&bytes);
    }

    pub fn len(&self) -> usize {
        return self.buf.len();
    }

    pub fn data(&self) -> &[u8] {
        return self.buf.as_slice();
    }
}

impl ReadBuffer {
    pub fn new(vec: Vec<u8>) -> ReadBuffer {
        ReadBuffer {
            buf: Cursor::new(vec),
        }
    }

    pub fn read_var_int(&mut self) -> u32 {
        return read_var_int(&mut self.buf);
    }

    pub fn read_byte(&mut self) -> u8 {
        let mut buf = [0; 1];
        self.read_bytes(&mut buf);
        return buf[0];
    }

    pub fn decompress(&mut self) {
        unimplemented!();
    }

    fn read_bytes(&mut self, bytes: &mut [u8]) {
        self.buf
            .read_exact(bytes)
            .expect("failed to read from buffer");
    }
}
