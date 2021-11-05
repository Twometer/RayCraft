pub struct McBuffer {
    buf: Vec<u8>,
    idx: u32,
}

impl McBuffer {
    pub fn new() -> McBuffer {
        McBuffer {
            buf: Vec::new(),
            idx: 0,
        }
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

    pub fn len(&self) -> usize {
        return self.buf.len();
    }

    pub fn write_buf(&mut self, other: &McBuffer) {
        self.buf.extend(&other.buf);
    }

    pub fn data(&self) -> &[u8] {
        return self.buf.as_slice();
    }

    fn write_bytes(&mut self, bytes: &[u8]) {
        self.buf.extend_from_slice(&bytes);
    }
}
