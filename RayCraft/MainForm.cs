using Craft.Net.Auth;
using RayCraft.Game;
using RayCraft.Renderer;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace RayCraft
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            RayCraftGame.Instance.Connect(new SessionToken("", UsernameBox.Text, "", ""), ServerBox.Text, 25565);
            LoginPane.Visible = false;
            BeginRendering();
        }

        private void BeginRendering()
        {
            Thread.Sleep(1000);
            var renderer = new WorldRenderer(ClientRectangle.Width, ClientRectangle.Height);
            Cursor.Hide();
            var center_c = new Point(Width / 2, Height / 2);
            var center_s = PointToScreen(center_c);
            var lastFrame = DateTime.Now;
            Thread renderThread = new Thread(() =>
            {
                Stopwatch watch = new Stopwatch();
                while (true)
                {
                    var deltatime = (float)(DateTime.Now - lastFrame).TotalSeconds * 5;
                    lastFrame = DateTime.Now;

                    watch.Start();
                    var img = renderer.RenderWorld();
                    if (IsDisposed)
                    {
                        Environment.Exit(0);
                        return;
                    }
                    Invoke((MethodInvoker)(delegate
                    {
                        BackgroundImage = (Image)img.Clone();
                        long fps = watch.ElapsedMilliseconds;
                        var p = PointToClient(MousePosition);

                        var dx = (p.X - center_c.X) / 2;
                        var dy = (p.Y - center_c.Y) / 2;
                        StatsLabel.Text = "FrameTime = " + watch.ElapsedMilliseconds.ToString() + "ms (" + (1000 / fps) + " fps)      " + dx + "      " + dy;
                        RayCraftGame.Instance.Player.Yaw += (dx * 2) * deltatime;
                        RayCraftGame.Instance.Player.Pitch += (dy * 2) * deltatime;
                        if (RayCraftGame.Instance.Player.Pitch > 90) RayCraftGame.Instance.Player.Pitch = 90;
                        if (RayCraftGame.Instance.Player.Pitch < -90) RayCraftGame.Instance.Player.Pitch = -90;
                        if (w_down)
                        {
                            RayCraftGame.Instance.Player.PosX += -Math.Sin(RenderMath.ToRadians(-RayCraftGame.Instance.Player.Yaw)) * deltatime;
                            RayCraftGame.Instance.Player.PosZ += -Math.Cos(RenderMath.ToRadians(-RayCraftGame.Instance.Player.Yaw)) * deltatime;
                        }
                        if (s_down)
                        {
                            RayCraftGame.Instance.Player.PosX += Math.Sin(RenderMath.ToRadians(-RayCraftGame.Instance.Player.Yaw)) * deltatime;
                            RayCraftGame.Instance.Player.PosZ += Math.Cos(RenderMath.ToRadians(-RayCraftGame.Instance.Player.Yaw)) * deltatime;
                        }
                        if (a_down)
                        {
                            RayCraftGame.Instance.Player.PosX += Math.Sin(RenderMath.ToRadians(-(RayCraftGame.Instance.Player.Yaw + 90))) * deltatime;
                            RayCraftGame.Instance.Player.PosZ += Math.Cos(RenderMath.ToRadians(-(RayCraftGame.Instance.Player.Yaw + 90))) * deltatime;
                        }
                        if (d_down)
                        {
                            RayCraftGame.Instance.Player.PosX += Math.Sin(RenderMath.ToRadians(-(RayCraftGame.Instance.Player.Yaw - 90))) * deltatime;
                            RayCraftGame.Instance.Player.PosZ += Math.Cos(RenderMath.ToRadians(-(RayCraftGame.Instance.Player.Yaw - 90))) * deltatime;
                        }
                        if (shift_down) RayCraftGame.Instance.Player.PosY -= deltatime;
                        if (space_down) RayCraftGame.Instance.Player.PosY += deltatime;
                        Cursor.Position = center_s;
                    }));

                    watch.Reset();
                }
            });
            renderThread.Start();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }

        private bool w_down;
        private bool a_down;
        private bool s_down;
        private bool d_down;
        private bool space_down;
        private bool shift_down;

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.W) w_down = true;
            else if (e.KeyCode == Keys.A) a_down = true;
            else if (e.KeyCode == Keys.S) s_down = true;
            else if (e.KeyCode == Keys.D) d_down = true;
            else if (e.KeyCode == Keys.Space) space_down = true;
            else if (e.Shift) shift_down = true;
        }

        private void MainForm_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.W) w_down = false;
            else if (e.KeyCode == Keys.A) a_down = false;
            else if (e.KeyCode == Keys.S) s_down = false;
            else if (e.KeyCode == Keys.D) d_down = false;
            else if (e.KeyCode == Keys.Space) space_down = false;
            else if (!e.Shift) shift_down = false;
        }
    }
}
