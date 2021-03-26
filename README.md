# RayCraft

Minecraft 1.8 Client and renderer using pure WinForms. 

Please note that this is just a stupid experiment of mine, and should not be used as a reference on raycasting or Minecraft Gameplay :-)

## How it works

The client connects to actual Minecraft servers (version 1.8.x compatible). The scene is rendered on the CPU using raycasting and drawn to the screen using WinForms and GDI+. In a small window (400px x 400px) and depending on the scene it runs in realtime (~30fps) on a `Ryzen 7 3800X`.

Scenes with open space (lots of sky showing) tend to be a lot slower than those with blocks in all directions, because to hit sky, the rays have to travel a lot further. As an optimization, the ray step size increases the further it travels away from the camera. This causes a lot of artifacting, but makes it run at playable speeds.

## How it looks

In short: Horrible

![image-20210326160407200](C:\Users\twome\AppData\Roaming\Typora\typora-user-images\image-20210326160407200.png)

