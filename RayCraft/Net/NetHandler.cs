using Craft.Net.Auth;
using Craft.Net.Crypto;
using Craft.Net.Packets.Login;
using Craft.Net.Packets.Play;
using System.Security.Cryptography;
using System;
using Craft.Client.World.Entities;
using Craft.Client;
using Craft.Client.World;
using Craft.Net.Util;
using RayCraft.Utils;
using RayCraft.Game;

namespace Craft.Net
{
    public class NetHandler
    {
        public MCNetworkManager manager;

        public void HandleS2FSlotUpdate(S2FSlotUpdate s2FSlotUpdate)
        {
            /*if(s2FSlotUpdate.windowId == 0)
            {
                CraftGame.GetCraft().Player.Inventory.UpdateSlot(s2FSlotUpdate.slotId, s2FSlotUpdate.stack);
            }*/
        }

        public void HandleS0CPlayerJoin(S0CPlayerJoin s0CPlayerJoin)
        {
            /*EntityOtherPlayer entity = new EntityOtherPlayer();
            entity.EntityId = s0CPlayerJoin.playerId;
            entity.SetPosition(s0CPlayerJoin.x, s0CPlayerJoin.y, s0CPlayerJoin.z);
            CraftGame.GetCraft().World.AddEntity(entity);*/
        }

        public bool waitingForEncryption;

        public NetHandler(MCNetworkManager manager)
        {
            this.manager = manager;
        }

        internal void HandleS22MultiBlockUpdate(S22MultiBlockUpdate s22MultiBlockUpdate)
        {
            Chunk chunk = RayCraftGame.Instance.World.GetChunk(s22MultiBlockUpdate.chunkX, s22MultiBlockUpdate.chunkZ);
            if (chunk != null)
            {
                foreach (BlockUpdate update in s22MultiBlockUpdate.updates)
                {
                    chunk.SetBlock(update.relativeX, update.y, update.relativeZ, (byte)update.newBlockId);
                }
            }
        }

        internal void HandleS23BlockUpdate(S23BlockUpdate s23BlockUpdate)
        {
            RayCraftGame.Instance.World.SetBlock(s23BlockUpdate.blockPos.X, s23BlockUpdate.blockPos.Y, s23BlockUpdate.blockPos.Z, s23BlockUpdate.newId);
        }

        internal void HandleS00KeepAlive(S00KeepAlive s00KeepAlive)
        {
            manager.SendPacket(new C00KeepAlive(s00KeepAlive.id));
        }

        internal void HandleS00LoginRejected(S00LoginRejected s00LoginRejected)
        {
            Logger.Error("Login rejected: " + s00LoginRejected.reason);
        }

        internal void HandleS01EncryptionRequest(S01EncryptionRequest s01EncryptionRequest)
        {
            waitingForEncryption = true;
            Logger.Info("Server is in online mode!");
            RSACryptoServiceProvider rsaProvider = CryptoHandler.DecodeRSAPublicKey(s01EncryptionRequest.serverKey);
            byte[] secretKey = CryptoHandler.GenerateAESPrivateKey();

            Logger.Info("Keys generated");
            if (s01EncryptionRequest.serverId != "-")
            {
                Logger.Info("Logging in to Mojang");
                if (!Yggdrasil.SessionCheck(manager.loginParams.UUID, manager.loginParams.SessionId, CryptoHandler.getServerHash(s01EncryptionRequest.serverId, s01EncryptionRequest.serverKey, secretKey)))
                {
                    Logger.Error("Mojang authentication failed");
                    return;
                }
                else
                {
                    Logger.Info("Mojang authentication succesful");
                }
            }

            manager.SendPacket(new C01EncryptionResponse(rsaProvider.Encrypt(secretKey, false), rsaProvider.Encrypt(s01EncryptionRequest.token, false)));

            manager.aesStream = CryptoHandler.getAesStream(manager.tcpClient.GetStream(), secretKey);
            manager.encrypted = true;
        }

        internal void HandleS01JoinCompleted(S01JoinCompleted s01JoinCompleted)
        {
            EntityPlayer Player = RayCraftGame.Instance.Player;
            Player.EntityId = s01JoinCompleted.playerEid;
            Player.Gamemode = s01JoinCompleted.gamemode;
            Player.Hardcore = s01JoinCompleted.hardcore;
        }

        internal void HandleS02ChatMessage(S02ChatMessage s02ChatMessage)
        {
            Logger.Info("Chat Message: " + s02ChatMessage.jsonMessage);
        }

        internal void HandleS02LoginSuccesful(S02LoginSuccessful s02LoginSuccessful)
        {
            if (!waitingForEncryption)
            {
                Logger.Info("Server is in offline mode!");
            }
            Logger.Info("Authentication successful");
            manager.gameState = GameState.Play;
        }

        internal void HandleS03Compression(S03Compression s03Compression)
        {
            this.manager.compressionThreshold = s03Compression.threshold;
        }

        internal void HandleS06HealthChanged(S06HealthChanged s06HealthChanged)
        {
            if (s06HealthChanged.health <= 0)
            {
                manager.SendPacket(new C16ClientStatus()); // Respawn
            }
            RayCraftGame.Instance.Player.Health = s06HealthChanged.health;
        }

        internal void HandleS08PlayerPosition(S08PlayerPosition s08PlayerPosition)
        {
            RayCraftGame.Instance.Player.SetPosition(s08PlayerPosition.x, s08PlayerPosition.y, s08PlayerPosition.z);
            manager.SendPacket(new C04PlayerPosition(s08PlayerPosition.x, s08PlayerPosition.y, s08PlayerPosition.z, true));
        }

        internal void HandleS12Velocity(S12Velocity s12Velocity)
        {
        }

        internal void HandleS18EntityTeleport(S18EntityTeleport s18EntityTeleport)
        {
        }

        internal void HandleS21MapData(S21MapData s21MapData)
        {
            if (s21MapData.Delete) RayCraftGame.Instance.World.DestroyChunk(s21MapData.DeletedX, s21MapData.DeletedZ);
            else RayCraftGame.Instance.World.AddChunk(s21MapData.Chunk);

        }
        internal void HandleS26MapChunkBulk(S26MapChunkBulk s26MapChunkBulk)
        {
            foreach (var ch in s26MapChunkBulk.chunks)
                RayCraftGame.Instance.World.AddChunk(ch);
            s26MapChunkBulk.chunks.Clear();
        }

        internal void HandleS40Disconnect(S40Disconnect s40Disconnect)
        {
            Logger.Info("Lost connection to server: " + s40Disconnect.jsonReason);
        }
    }
}