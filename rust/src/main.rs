mod mc;

fn main() {
    let mut client = mc::Client::connect("localhost:25565").expect("Failed to connect");
    client.login("Rustacean");
    loop {}
}
