using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;
using System.Windows;
using Orbit.Core.Players;
using Orbit.Core.AI;
using Orbit.Core.Helpers;
using Orbit.Core.Scene.Entities.Implementations;
using System.Windows.Controls;
using Orbit.Core.Scene.Entities;
using Orbit.Core.Scene.Controls.Implementations;
using System.Windows.Media;
using Orbit.Core.Weapons;
using Orbit.Core.Players.Input;

namespace Orbit.Core.Client
{
    public partial class SceneMgr
    {
        private void ConnectToServer()
        {
            NetOutgoingMessage msg = client.CreateMessage();
            msg.Write((int)PacketType.PLAYER_CONNECT);
            msg.Write(GetCurrentPlayer().Data.Name);
            msg.Write(GetCurrentPlayer().Data.HashId);

            serverConnection = client.Connect(serverAddress, SharedDef.PORT_NUMBER, msg);
        }

        private void ClientConnectionConnected(NetIncomingMessage msg)
        {
            currentPlayer.Data.Id = msg.SenderConnection.RemoteHailMessage.ReadInt32();
            // pokud uz takove jmeno na serveru existuje, tak obdrzime nove
            currentPlayer.Data.Name = msg.SenderConnection.RemoteHailMessage.ReadString();
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                (Application.Current as App).PlayerName = currentPlayer.Data.Name;
            }));
            // pokud je hra zakladana pres lobby, tak o tom musi vedet i klient, ktery ji nezakladal
            Gametype serverType = (Gametype)msg.SenderConnection.RemoteHailMessage.ReadByte();
            tournametRunnig = msg.SenderConnection.RemoteHailMessage.ReadBoolean();
            if (GameType != Gametype.TOURNAMENT_GAME && serverType == Gametype.TOURNAMENT_GAME)
            {
                GameType = serverType;
                if (!tournametRunnig)
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        (Application.Current as App).CreateLobbyGui(false);
                    }));
            }

            while (pendingMessages.Count != 0)
                SendMessage(pendingMessages.Dequeue());

            Console.WriteLine("LOGIN confirmed (id: " + IdMgr.GetHighId(GetCurrentPlayer().Data.Id) + ")");

            SendPlayerDataRequestMessage();
        }

        private void SendPlayerDataRequestMessage()
        {
            NetOutgoingMessage reqmsg = CreateNetMessage();
            reqmsg.Write((int)PacketType.ALL_PLAYER_DATA_REQUEST);
            SendMessage(reqmsg);
        }

        public void SendStartGameRequest()
        {
            NetOutgoingMessage msg = CreateNetMessage();
            msg.Write((int)PacketType.START_GAME_REQUEST);
            SendMessage(msg);
        }

        public void SendPlayerReadyMessage()
        {
            NetOutgoingMessage msg = CreateNetMessage();
            msg.Write((int)PacketType.PLAYER_READY);
            msg.Write(currentPlayer.GetId());
            msg.Write(currentPlayer.Data.LobbyLeader);
            SendMessage(msg);
        }

        private void ReceivedAllPlayerDataMsg(NetIncomingMessage msg)
        {
            int playerCount = msg.ReadInt32();
            List<int> newPlrs = new List<int>(playerCount);

            for (int i = 0; i < playerCount; ++i)
            {
                Player plr = GetPlayer(msg.ReadInt32());
                // jeste hrace nezname
                if (plr == null)
                {
                    plr = CreatePlayer();
                    players.Add(plr);

                    msg.ReadObjectPlayerData(plr.Data);

                    if (plr.Data.PlayerType == PlayerType.BOT)
                        CreateAndAddBot(plr);
                    else
                        FloatingTextMgr.AddFloatingText(plr.Data.Name + " has joined the game",
                            new Vector(SharedDef.VIEW_PORT_SIZE.Width / 2, SharedDef.VIEW_PORT_SIZE.Height / 2 - 50 + i * 30),
                            FloatingTextManager.TIME_LENGTH_5, FloatingTextType.SYSTEM, FloatingTextManager.SIZE_MEDIUM, true);
                }
                else // hrace uz zname, ale mohl se zmenit jeho stav na active a take se mohly zmenit dalsi player data
                    msg.ReadObjectPlayerData(plr.Data);

                newPlrs.Add(plr.GetId());
            }

            // pokud mame navic nejake stare hrace, tak je odstranime
            if (playerCount != newPlrs.Count)
                for (int i = 0; i < players.Count; ++i)
                    if (!newPlrs.Contains(players[i].GetId()))
                        players.RemoveAt(i);

            if ((GameType != Gametype.TOURNAMENT_GAME || tournametRunnig) && !currentPlayer.Data.StartReady)
                SendStartGameRequest();
            else
            {
                CheckAllPlayersReady();
                UpdateLobbyPlayers();
            }
        }

        private void ReceivedServerShuttingDownMsg(NetIncomingMessage msg)
        {
            EndGame(null, GameEnd.SERVER_DISCONNECTED);
        }

        private void ReceivedPlayerDisconnectedMsg(NetIncomingMessage msg)
        {
            Player disconnected = GetPlayer(msg.ReadInt32());

            FloatingTextMgr.AddFloatingText(disconnected.Data.Name + " has disconnected",
                new Vector(SharedDef.VIEW_PORT_SIZE.Width / 2, SharedDef.VIEW_PORT_SIZE.Height / 2 - 50),
                FloatingTextManager.TIME_LENGTH_5, FloatingTextType.SYSTEM, FloatingTextManager.SIZE_MEDIUM, true);

            players.Remove(disconnected);
            (Application.Current as App).SetGameStarted(false);

            if (disconnected.IsActivePlayer())
                EndGame(disconnected, GameEnd.LEFT_GAME);

            if (GameType == Gametype.TOURNAMENT_GAME)
            {
                UpdateLobbyPlayers();
                CheckAllPlayersReady();
            }
        }

        private void ReceivedTournamentFinishedMgs(NetIncomingMessage msg)
        {
            EndGame(GetPlayer(msg.ReadInt32()), GameEnd.TOURNAMENT_FINISHED);
        }

        private void ReceivedTournamentStartingMsg(NetIncomingMessage msg)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                (Application.Current as App).CreateGameGui(false);
                (Application.Current as App).SetGameStarted(true);
                SetCanvas((Application.Current as App).GetCanvas());
            }));

            InitStaticMouse();
            BeginInvoke(new Action(() =>
            {
                Label lbl = (Label)LogicalTreeHelper.FindLogicalNode(canvas, "lblEndGame");
                if (lbl != null)
                    lbl.Content = "";

                Label lblw = (Label)LogicalTreeHelper.FindLogicalNode(canvas, "lblWaiting");
                if (lblw != null)
                    lblw.Content = "";

            }));
        }

        private void ReceivedStartGameResponseMsg(NetIncomingMessage msg)
        {
            string leftPlr = players.Find(p => p.IsActivePlayer() && p.GetPosition() == PlayerPosition.LEFT).Data.Name;
            string rightPlr = players.Find(p => p.IsActivePlayer() && p.GetPosition() == PlayerPosition.RIGHT).Data.Name;

            InitStaticMouse();
            (Application.Current as App).SetGameStarted(true);

            foreach (Player p in players)
            {
                if (p.IsActivePlayer())
                {
                    p.CreateWeapons();
                    if (p.IsCurrentPlayer())
                    {
                        actionBarMgr = new ActionBarMgr(this);
                        StateMgr.AddGameState(actionBarMgr);
                        actionBarMgr.CreateActionBarItems();
                    }

                    // zobrazi aktualni integrity bazi
                    p.SetBaseIntegrity(p.GetBaseIntegrity());
                    p.Baze = SceneObjectFactory.CreateBase(this, p);
                    DelayedAttachToScene(p.Baze);
                }

                if (p.IsCurrentPlayer())
                {
                    if (p.IsActivePlayer())
                        inputMgr = new PlayerInputMgr(p, this);
                    else
                        inputMgr = new SpectatorInputMgr(p, this);
                }
            }

            Invoke(new Action(() =>
            {
                Label lbl3 = (Label)LogicalTreeHelper.FindLogicalNode(canvas, "lblNameLeft");
                if (lbl3 != null)
                    lbl3.Content = leftPlr;

                Label lbl4 = (Label)LogicalTreeHelper.FindLogicalNode(canvas, "lblNameRight");
                if (lbl4 != null)
                    lbl4.Content = rightPlr;
            }));

            if (currentPlayer.IsActivePlayer())
                ShowStatusText(3, "You are " + (currentPlayer.GetPlayerColor() == Colors.Red ? "Red" : "Blue"));
            else
                ShowStatusText(3, "You are Spectator");

            SetMainInfoText("");
            userActionsDisabled = false;
        }

        private void ReceivedPlayerAndGoldScoreMsg(NetIncomingMessage msg)
        {
            Player p = GetPlayer(msg.ReadInt32());
            if (p != null && !p.IsCurrentPlayer())
            {
                p.Data.Score = msg.ReadInt32();
                p.Data.Gold = msg.ReadInt32();
            }
        }

        private void ReceivedBaseIntegrityChangeMsg(NetIncomingMessage msg)
        {
            GetPlayer(msg.ReadInt32()).SetBaseIntegrity(msg.ReadInt32());
        }

        private void ReceivedPlayerHealMsg(NetIncomingMessage msg)
        {
            GetPlayer(msg.ReadInt32()).SetBaseIntegrity(msg.ReadInt32(), true);
        }

        private void ReceivedBulletHitMsg(NetIncomingMessage msg)
        {
            long bulletId = msg.ReadInt64();
            long aId = msg.ReadInt64();
            int damage = msg.ReadInt32();

            IProjectile bullet = null;
            IDestroyable target = null;

            foreach (ISceneObject obj in objects)
            {
                if (obj.Id == bulletId)
                {
                    if (obj is IProjectile)
                        bullet = obj as IProjectile;

                    if (obj is SingularityBullet)
                        bullet.DoRemoveMe();

                    break;
                }
            }

            foreach (ISceneObject obj in objects)
            {
                if (obj.Id != aId)
                    continue;

                if (obj is IDestroyable)
                    target =(obj as IDestroyable);
                else
                    Console.Error.WriteLine("Object id " + bulletId + " (" + obj.GetType().Name + ") is supposed to be a instance of IDestroyable but it is not");

                break;
            }

            if (target != null)
                target.TakeDamage(damage, bullet);
            else
                idsToRemove.Add(aId);
        }

        private void ReceivedHookHitMsg(NetIncomingMessage msg)
        {
            long hookId = msg.ReadInt64();
            long asteroidId = msg.ReadInt64();
            Vector position = msg.ReadVector();
            Vector hitVector = msg.ReadVector();
            Hook hook = null;
            ICatchable g = null;
            foreach (ISceneObject obj in objects)
            {
                if (obj.Id == hookId)
                {
                    if (obj is Hook)
                    {
                        hook = obj as Hook;
                        if (g != null)
                        {
                            hook.Catch(g, hitVector);
                            break;
                        }
                        else
                            continue;
                    }
                    else
                        Console.Error.WriteLine("Object id " + hookId + " (" + obj.GetType().Name + ") is supposed to be a Hook but it is not");
                }

                if (obj.Id == asteroidId)
                {
                    if (obj is ICatchable)
                    {
                        g = obj as ICatchable;
                        obj.Position = position;
                        if (hook != null)
                        {
                            hook.Catch(g, hitVector);
                            break;
                        }
                    }
                    else
                        Console.Error.WriteLine("Object id " + asteroidId + " (" + obj.GetType().Name + ") is supposed to be a IContainsGold but it is not");
                }

            }
        }

        private void ReceivedSingularityMineHitMsg(NetIncomingMessage msg)
        {
            long mineId = msg.ReadInt64();
            long id = msg.ReadInt64();
            Vector pos = msg.ReadVector();
            Vector dir = msg.ReadVector();
            foreach (ISceneObject obj in objects)
            {
                if (obj.Id == mineId)
                {
                    DroppingSingularityControl c = obj.GetControlOfType(typeof(DroppingSingularityControl)) as DroppingSingularityControl;
                    if (c == null)
                        Console.Error.WriteLine("Object id " + mineId + " (" + obj.GetType().Name + ") is supposed to be a SingularityMine and have DroppingSingularityControl, but control is null");
                    else
                        c.StartDetonation();
                    continue;
                }

                if (obj.Id != id)
                    continue;

                obj.Position = pos;
                (obj as IMovable).Direction += dir;
            }
        }

        private void ReceivedPlayerWonMsg(NetIncomingMessage msg)
        {
            Player winner = GetPlayer(msg.ReadInt32());
            winner.Data.WonMatches = msg.ReadInt32();
            EndGame(winner, GameEnd.WIN_GAME);
        }

        private void ReceivedScoreQueryMsg(NetIncomingMessage msg)
        {
            if (!currentPlayer.IsActivePlayer())
                return;

            isGameInitialized = false;
            NetOutgoingMessage scoreMsg = CreateNetMessage();
            scoreMsg.Write((int)PacketType.SCORE_QUERY_RESPONSE);
            scoreMsg.Write(currentPlayer.GetId());
            scoreMsg.Write(currentPlayer.Data.Score);
            SendMessage(scoreMsg);
        }

        private void ReceivedNewHookMsg(NetIncomingMessage msg)
        {
            Hook h = new Hook(this);
            h.ReadObject(msg);
            h.Owner = GetOpponentPlayer();
            h.SetGeometry(SceneGeometryFactory.CreateHookHead(h));
            BeginInvoke(new Action(() =>
            {
                Canvas.SetZIndex(h.GetGeometry(), 99);
            }));
            h.PrepareLine();
            DelayedAttachToScene(h);
            SyncReceivedObject(h, msg);
        }

        private void ReceiveNewLaserMsg(NetIncomingMessage msg)
        {
            Laser laser = new Laser(GetOpponentPlayer(), this);
            laser.ReadObject(msg);

            DelayedAttachToScene(laser);
            SyncReceivedObject(laser, msg);
        }

        private void ReceivedNewSingularityExplodingBulletMsg(NetIncomingMessage msg)
        {
            SingularityExplodingBullet s = new SingularityExplodingBullet(this);
            s.ReadObject(msg);
            s.Owner = GetOpponentPlayer();
            s.SetGeometry(SceneGeometryFactory.CreateConstantColorEllipseGeometry(s));
            DelayedAttachToScene(s);
            SyncReceivedObject(s, msg);
        }

        private void ReceivedNewSingularityBulletMsg(NetIncomingMessage msg)
        {
            SingularityBullet s = new SingularityBullet(this);
            s.ReadObject(msg);
            s.Owner = GetOpponentPlayer();
            s.SetGeometry(SceneGeometryFactory.CreateConstantColorEllipseGeometry(s));
            DelayedAttachToScene(s);
            SyncReceivedObject(s, msg);
        }

        private void ReceivedNewSingularityMineMsg(NetIncomingMessage msg)
        {
            SingularityMine s = new SingularityMine(this);
            s.ReadObject(msg);
            s.Owner = GetOpponentPlayer();
            s.SetGeometry(SceneGeometryFactory.CreateRadialGradientEllipseGeometry(s));
            DelayedAttachToScene(s);
            SyncReceivedObject(s, msg);
        }

        private void ReceivedMinorAsteroidSpawnMsg(NetIncomingMessage msg)
        {
            int radius = msg.ReadInt32();
            Vector direction = msg.ReadVector();
            Vector center = msg.ReadVector();
            int rot = msg.ReadInt32();
            int textureId = msg.ReadInt32();
            int destoryerId = msg.ReadInt32();
            long id1 = msg.ReadInt64();
            long id2 = msg.ReadInt64();
            long id3 = msg.ReadInt64();

            MinorAsteroid a1 = SceneObjectFactory.CreateSmallAsteroid(this, id1, direction, center, rot, textureId, radius, Math.PI / 12);
            MinorAsteroid a2 = SceneObjectFactory.CreateSmallAsteroid(this, id2, direction, center, rot, textureId, radius, 0);
            MinorAsteroid a3 = SceneObjectFactory.CreateSmallAsteroid(this, id3, direction, center, rot, textureId, radius, -Math.PI / 12);

            UnstableAsteroid p = new UnstableAsteroid(this);
            p.Destroyer = destoryerId;

            a1.Parent = p;
            a2.Parent = p;
            a3.Parent = p;

            DelayedAttachToScene(a1);
            DelayedAttachToScene(a2);
            DelayedAttachToScene(a3);

            SyncReceivedObject(a1, msg);
            SyncReceivedObject(a2, msg);
            SyncReceivedObject(a3, msg);
        }


        private void ReceivedNewAsteroidMsg(NetIncomingMessage msg)
        {
            Asteroid s = CreateNewAsteroid((AsteroidType)msg.ReadByte());
            s.ReadObject(msg);
            s.SetGeometry(SceneGeometryFactory.CreateAsteroidImage(s));
            DelayedAttachToScene(s);
            SyncReceivedObject(s, msg);
        }

        private void ReceivedNewStatPowerupMsg(NetIncomingMessage msg)
        {
            StatPowerUp p = new StatPowerUp(this);
            p.ReadObject(msg);
            p.SetGeometry(SceneGeometryFactory.CreatePowerUpImage(p));
            DelayedAttachToScene(p);
            SyncReceivedObject(p, msg);
        }

        private void ReceivedPlayerReceivedPowerUpMsg(NetIncomingMessage msg)
        {
            StatsMgr.AddStatToPlayer(GetPlayer(msg.ReadInt32()).Data, (PlayerStats)msg.ReadByte(), msg.ReadFloat());
        }

        private void ReceivedPlayerReadyMsg(NetIncomingMessage msg)
        {
            Player pl = GetPlayer(msg.ReadInt32());
            pl.Data.LobbyReady = true;
            CheckAllPlayersReady();
        }

        private void ReceivedAllAsteroidsMsg(NetIncomingMessage msg)
        {
            if (objects.Count > 2)
            {
                Console.WriteLine("Error: receiving all asteroids but already have " + objects.Count);
                return;
            }

            int count = msg.ReadInt32();
            for (int i = 0; i < count; ++i)
            {
                if (msg.ReadInt32() != (int)PacketType.NEW_ASTEROID)
                {
                    Console.WriteLine("Corrupted object PacketType.SYNC_ALL_ASTEROIDS");
                    return;
                }
                Asteroid s = CreateNewAsteroid((AsteroidType)msg.ReadByte());
                s.ReadObject(msg);
                s.SetGeometry(SceneGeometryFactory.CreateAsteroidImage(s));
                DelayedAttachToScene(s);
                SyncReceivedObject(s, msg);
            }
            isGameInitialized = true;
        }

        private void ReceivedPlayerBoughtUpgradeMsg(NetIncomingMessage msg)
        {
            Player p = GetPlayer(msg.ReadInt32());
            switch ((DeviceType)msg.ReadByte())
            {
                case DeviceType.MINE:
                    p.Mine = p.Mine.Next();
                    break;
                case DeviceType.CANNON:
                    p.Canoon = p.Canoon.Next();
                    break;
                case DeviceType.HOOK:
                    p.Hook = p.Hook.Next();
                    break;
                case DeviceType.HEALING_KIT:
                    //p.HealingKit = p.HealingKit.Next();
                    break;
            }
        }

        private void ReceivedFloatingTextMsg(NetIncomingMessage msg)
        {
            FloatingTextMgr.AddFloatingText(msg.ReadString(), msg.ReadVector(), msg.ReadFloat(), (FloatingTextType)msg.ReadByte(), msg.ReadFloat(), true, false);
        }

        private void RecieveMoveLaserObject(NetIncomingMessage msg)
        {
            long id = msg.ReadInt64();
            Point end = msg.ReadPoint();
            foreach (ISceneObject obj in objects)
            {
                if (obj.Id == id && obj is Laser)
                {
                    (obj as Laser).End = end;
                    break;
                }
            }
        }

        private void ReceiveRemoveObject(NetIncomingMessage msg)
        {
            long id = msg.ReadInt64();
            ISceneObject toRemove = null;
            foreach (ISceneObject obj in objects)
            {
                if (obj.Id == id)
                {
                    toRemove = obj;
                    break;
                }
            }

            if (toRemove != null)
                RemoveFromSceneDelayed(toRemove);
            else
                idsToRemove.Add(id);
        }
    }
}
