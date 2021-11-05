use crate::buffer::McBuffer;

mod buffer;
mod minecraft;

fn main() {
    println!("Twometer's Minecraft Client in Rust");
    let mut client = minecraft::McClient::connect().expect("Failed to connect");
    client.send_handshake("localhost", 25565, 2);
    client.send_login("Rustacean");
    loop {}
}
