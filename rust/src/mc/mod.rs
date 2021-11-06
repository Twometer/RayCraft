// Import the submodules
pub mod buffer;
pub mod client;

use std::io::Read;

// Export the public types
pub use self::buffer::ReadBuffer;
pub use self::buffer::WriteBuffer;
pub use self::client::Client;

// Common functions
fn read_var_int(source: &mut impl Read) -> u32 {
    let mut val: u32 = 0;
    let mut buf = [0; 1];
    for i in 0..4 {
        source.read_exact(&mut buf).expect("failed to read VarInt");

        let masked = (buf[0] & 0x7f) as u32;
        val |= masked << i * 7;

        if buf[0] & 0x80 == 0 {
            break;
        }
    }
    return val;
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
