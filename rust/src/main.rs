use crate::buffer::McBuffer;

mod buffer;
mod minecraft;

fn main() {
    println!("Twometer's Minecraft Client in Rust");
    let mut client = minecraft::McClient::connect("localhost:25565").expect("Failed to connect");
    client.login("Rustacean");
    loop {}
}
