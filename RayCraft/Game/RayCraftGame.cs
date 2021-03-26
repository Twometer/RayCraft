using Craft.Client.World;
using Craft.Client.World.Entities;
using Craft.Net;
using Craft.Net.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayCraft.Game
{
    public class RayCraftGame
    {
        public static RayCraftGame Instance { get; } = new RayCraftGame();

        public World World { get; private set; }
        public EntityPlayer Player { get; set; }
        public MCNetworkManager NetworkManager { get; private set; }
        public SessionToken SessionToken { get; private set; }

        public RayCraftGame()
        {
            World = new World();
            Player = new EntityPlayer();
            NetworkManager = new MCNetworkManager();
        }

        public void Connect(SessionToken token, string server, ushort port)
        {
            LoginParams lParams = new LoginParams()
            {
                ServerHost = server,
                ServerPort = port,
                Username = token.PlayerName,
                UUID = token.PlayerID,
                SessionId = token.ID
            };
            SessionToken = token;
            NetworkManager.Login(lParams);
        }

    }
}
