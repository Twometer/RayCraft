mod mcnet;

fn main() {
    let mut client = mcnet::Client::connect("localhost:25565").expect("Failed to connect");
    client.login("Rustacean");
    loop {}
}
