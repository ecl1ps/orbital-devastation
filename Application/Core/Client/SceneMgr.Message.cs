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
using Orbit.Core.Scene.Controls;
using Orbit.Core.Client.GameStates;
using Orbit.Core.Scene.Controls.Health.Implementations;
using Orbit.Core.Scene.Controls.Health;
using Orbit.Gui;
using Orbit.Gui.ActionControllers;
using Orbit.Core.SpecialActions.Spectator;
using Orbit.Core.SpecialActions;
using Orbit.Core.SpecialActions.Gamer;
using Orbit.Core.Scene.Entities.Implementations.HeavyWeight;

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
            msg.Write(GetCurrentPlayer().Data.PlayerColor);

            serverConnection = client.Connect(serverAddress, SharedDef.PORT_NUMBER, msg);
        }

        private void ClientConnectionConnected(NetIncomingMessage msg)
        {
            currentPlayer.Data.Id = msg.SenderConnection.RemoteHailMessage.ReadInt32();
            // pokud uz takove jmeno na serveru existuje, tak obdrzime nove
            currentPlayer.Data.Name = msg.SenderConnection.RemoteHailMessage.ReadString();
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                App.Instance.PlayerName = currentPlayer.Data.Name;
            }));
            // pokud je hra zakladana pres lobby, tak o tom musi vedet i klient, ktery ji nezakladal
            Gametype serverType = (Gametype)msg.SenderConnection.RemoteHailMessage.ReadByte();
            tournametRunnig = msg.SenderConnection.RemoteHailMessage.ReadBoolean();
            if (GameType != Gametype.TOURNAMENT_GAME && serverType == Gametype.TOURNAMENT_GAME)
            {
                GameType = serverType;
                if (!tournametRunnig)
                {
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        App.Instance.CreateLobbyGui(false);
                    }));
                    SendTournamentSettingsRequest();
                    SendChatMessage(GetCurrentPlayer().Data.Name + " joined the lobby", true);
                }
            }

            while (pendingMessages.Count != 0)
                SendMessage(pendingMessages.Dequeue());

            Logger.Debug("LOGIN confirmed (id: " + IdMgr.GetHighId(GetCurrentPlayer().Data.Id) + ")");

            SendPlayerDataRequestMessage();
        }

        private void SendTournamentSettingsRequest()
        {
            NetOutgoingMessage reqmsg = CreateNetMessage();
            reqmsg.Write((int)PacketType.TOURNAMENT_SETTINGS_REQUEST);
            SendMessage(reqmsg);
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

        public void SendPlayerReadyMessage(bool ready)
        {
            NetOutgoingMessage msg = CreateNetMessage();
            msg.Write((int)PacketType.PLAYER_READY);
            msg.Write(currentPlayer.GetId());
            msg.Write(ready);
            msg.Write(currentPlayer.Data.LobbyLeader);
            SendMessage(msg);
        }

        private void SendPlayerColorChanged()
        {
            NetOutgoingMessage msg = CreateNetMessage();
            msg.Write((int)PacketType.PLAYER_COLOR_CHANGED);
            msg.Write(currentPlayer.GetId());
            msg.Write(currentPlayer.GetPlayerColor());
            SendMessage(msg);
        }

        private void ReceivedActionScheduleMsg(NetIncomingMessage msg)
        {
            Player owner = GetPlayer(msg.ReadInt32());
            SpectatorActionMgr.ReceiveActionScheduleMessage(msg, owner);
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

                    plr.Data.ReadObject(msg);
                    plr.Statistics.ReadObject(msg);

                    if (plr.Data.PlayerType == PlayerType.BOT)
                        CreateAndAddBot(plr);
                    else
                        FloatingTextMgr.AddFloatingText(plr.Data.Name + " has joined the game",
                            new Vector(SharedDef.VIEW_PORT_SIZE.Width / 2, SharedDef.VIEW_PORT_SIZE.Height / 2 - 50),
                            FloatingTextManager.TIME_LENGTH_5, FloatingTextType.SYSTEM, FloatingTextManager.SIZE_MEDIUM, true);
                }
                else
                {
                    // hrace uz zname, ale mohl se zmenit jeho stav na active a take se mohly zmenit dalsi player data
                    plr.Data.ReadObject(msg);
                    plr.Statistics.ReadObject(msg);
                }

                newPlrs.Add(plr.GetId());
            }

            // pokud mame navic nejake stare hrace, tak je odstranime
            if (playerCount != newPlrs.Count)
                for (int i = 0; i < players.Count; ++i)
                    if (!newPlrs.Contains(players[i].GetId()))
                        players.RemoveAt(i);

            if ((GameType != Gametype.TOURNAMENT_GAME || tournametRunnig) && !currentPlayer.Data.StartReady)
                SendStartGameRequest();
            else if (GameWindowState == WindowState.IN_LOBBY)
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
            if (disconnected == null)
                return;

            if (GameType == Gametype.TOURNAMENT_GAME && !disconnected.IsActivePlayer())
            {
                FloatingTextMgr.AddFloatingText(disconnected.Data.Name + " has disconnected",
                    new Vector(SharedDef.VIEW_PORT_SIZE.Width / 2, SharedDef.VIEW_PORT_SIZE.Height / 2 - 50),
                    FloatingTextManager.TIME_LENGTH_5, FloatingTextType.SYSTEM, FloatingTextManager.SIZE_MEDIUM, true);
            }

            players.Remove(disconnected);
            App.Instance.SetGameStarted(false);

            if (disconnected.IsActivePlayer())
                EndGame(disconnected, GameEnd.LEFT_GAME);
            else if (disconnected.Device != null)
                disconnected.Device.DoRemoveMe();

            if (GameWindowState == WindowState.IN_LOBBY)
            {
                UpdateLobbyPlayers();
                CheckAllPlayersReady();
            }
        }

        private void ReceivedTournamentFinishedMgs(NetIncomingMessage msg)
        {
            Player winner = GetPlayer(msg.ReadInt32());

            for (int i = 0; i < 2; ++i)
            {
                Player playedLastGame = GetPlayer(msg.ReadInt32());
                if (playedLastGame != null)
                {
                    playedLastGame.Data.WonMatches = msg.ReadInt32();
                    playedLastGame.Data.PlayedMatches = msg.ReadInt32();
                }
                else
                {
                    msg.ReadInt32();
                    msg.ReadInt32();
                }
            }

            EndGame(winner, GameEnd.TOURNAMENT_FINISHED);
        }

        private void ReceivedTournamentStartingMsg(NetIncomingMessage msg)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                App.Instance.CreateGameGui(false);
                App.Instance.SetGameStarted(true);
                SetGameVisualArea(App.Instance.GetGameArea());
            }));

            BeginInvoke(new Action(() =>
            {
                Label lbl = (Label)LogicalTreeHelper.FindLogicalNode(area.Parent, "lblEndGame");
                if (lbl != null)
                    lbl.Content = "";

                Label lblw = (Label)LogicalTreeHelper.FindLogicalNode(area.Parent, "lblWaiting");
                if (lblw != null)
                    lblw.Content = "";

            }));
        }

        private void ReceivedStartGameResponseMsg(NetIncomingMessage msg)
        {
            Player leftPlr = players.Find(p => p.IsActivePlayer() && p.GetPosition() == PlayerPosition.LEFT);
            Player rightPlr = players.Find(p => p.IsActivePlayer() && p.GetPosition() == PlayerPosition.RIGHT);

            InitStaticMouse();
            AlertMessageMgr.InitElement();
            App.Instance.SetGameStarted(true);

            foreach (Player p in players)
                CreateActiveObjectsOfPlayer(p);

            if (lastTournamentSettings != null)
                lastTournamentSettings.PlayedMatches++;

            Invoke(new Action(() =>
            {
                if (leftPlr != null)
                {
                    Label lbl3 = (Label)LogicalTreeHelper.FindLogicalNode(area.Parent, "lblNameLeft");
                    if (lbl3 != null)
                        lbl3.Content = leftPlr.Data.Name;
                }

                if (rightPlr != null)
                {
                    Label lbl4 = (Label)LogicalTreeHelper.FindLogicalNode(area.Parent, "lblNameRight");
                    if (lbl4 != null)
                        lbl4.Content = rightPlr.Data.Name;
                }
            }));

            if (currentPlayer.IsActivePlayer())
                ShowStatusText(3, "You are on the " + (currentPlayer.GetPosition() == PlayerPosition.LEFT ? "left" : "right"));
            else
                ShowStatusText(3, "You are Spectator");

            SetMainInfoText("");
            userActionsDisabled = false;

            Application.Current.Dispatcher.BeginInvoke(new Action(() => {
                App.Instance.FocusWindow();
            }));
        }

        private void ReceivedPlayerReconnectedMsg(NetIncomingMessage msg)
        {
            Player reconnecting = GetPlayer(msg.ReadInt32());
            if (reconnecting == null)
            {
                Logger.Error("Unknown reconnecting player");
                return;
            }

            if (!reconnecting.IsActivePlayer() && currentPlayer.IsActivePlayer())
            {
                players.FindAll(p => p.IsActivePlayer()).ForEach(p =>
                {
                    if (p.Data.FriendlyPlayerId == p.GetId())
                        p.Data.PlayerColor = p.GetPlayerColor();
                });
            }

            CreateActiveObjectsOfPlayer(reconnecting);
        }

        /// <summary>
        /// vytvori hraci action bar, input manager a zbrane a bazi nebo mining module
        /// </summary>
        /// <param name="p">hrac kteremu se maji vytvorit objekty</param>
        private void CreateActiveObjectsOfPlayer(Player p)
        {
            if (p.IsActivePlayer())
            {
                p.CreateWeapons();

                // zobrazi aktualni integrity bazi
                p.SetBaseIntegrity(p.GetBaseIntegrity());
                p.Baze = SceneObjectFactory.CreateBase(this, p);

                BaseIntegrityBar ellipse = SceneObjectFactory.CreateBaseIntegrityBar(this, p);

                HpBarControl control = new HpBarControl(ellipse);
                p.Baze.AddControl(control);

                DelayedAttachToScene(ellipse);
                DelayedAttachToScene(p.Baze);
            }
            else
            {
                MiningModule obj = SceneObjectFactory.CreateMiningModule(this, p.Data.MiningModuleStartPos, p);
                DelayedAttachToScene(obj);
                DelayedAttachToScene(SceneObjectFactory.CreateMiningModuleIntegrityBar(this, obj, p));

                p.Device = obj;
            }

            if (p.IsCurrentPlayer())
            {
                actionBarMgr = new ActionBarMgr(this);
                StateMgr.AddGameState(actionBarMgr);

                if (p.IsActivePlayer())
                {
                    inputMgr = new PlayerInputMgr(p, this, actionBarMgr);
                    actionBarMgr.CreateActionBarItems(p.GetActions<IPlayerAction>(), false);
                }
                else
                {
                    MiningModuleControl mc = new MiningModuleControl();
                    mc.Owner = p;
                    p.Device.AddControl(mc);

                    inputMgr = new SpectatorInputMgr(p, this, p.Device, actionBarMgr);
                    actionBarMgr.CreateActionBarItems(p.GetActions<ISpectatorAction>(), true);
                }
            }
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

                    ExplodingSingularityBulletControl c = obj.GetControlOfType<ExplodingSingularityBulletControl>();
                    if (c != null)
                        c.StartDetonation();

                    break;
                }
            }

            foreach (ISceneObject obj in objects)
            {
                if (obj.Id != aId)
                    continue;

                if (obj is IDestroyable)
                    target = (obj as IDestroyable);
                else
                    Logger.Error("Object id " + bulletId + " (" + obj.GetType().Name + ") is supposed to be a instance of IDestroyable but it is not");

                break;
            }

            if (bullet == null)
                return;

            if (target != null)
                target.TakeDamage(damage, bullet);
            else
                idsToRemove.Add(aId);

            if (!(bullet is SingularityExplodingBullet))
                bullet.DoRemoveMe();
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
                    hook = obj as Hook;
                }
                else if (obj.Id == asteroidId)
                {
                    g = obj as ICatchable;
                    obj.Position = position;
                }

                if (g != null && hook != null && hook.Owner != currentPlayer)
                {
                    hook.GetControlOfType<HookControl>().CatchObject(g, hitVector);
                    break;
                }
            }
        }

        private void ReceivedSingularityMineHitMsg(NetIncomingMessage msg)
        {
            long mineId = msg.ReadInt64();
            long id = msg.ReadInt64();
            Vector pos = msg.ReadVector();
            Vector dir = msg.ReadVector();
            float speed = msg.ReadFloat();
            foreach (ISceneObject obj in objects)
            {
                if (obj.Id == mineId)
                {
                    DroppingSingularityControl c = obj.GetControlOfType<DroppingSingularityControl>();
                    if (c == null)
                    {
                        ExplodingSingularityBulletControl c2 = obj.GetControlOfType<ExplodingSingularityBulletControl>();
                        if (c2 == null)
                            Logger.Error("Object id " + mineId + " (" + obj.GetType().Name + 
                                ") is supposed to be a SingularityMine and have DroppingSingularityControl " +
                                "or SingularityExplodingBullet and have ExplodingSingularityBulletControl, but control is null");
                        else
                            c2.StartDetonation();
                    }
                    else
                        c.StartDetonation();
                    continue;
                }

                if (obj.Id != id)
                    continue;

                obj.Position = pos;
                (obj as IMovable).Direction = dir;

                IMovementControl control = obj.GetControlOfType<IMovementControl>();
                if (control != null)
                    control.Speed = speed;
            }

            //SoundManager.Instance.StartPlayingOnce(SharedDef.MUSIC_EXPLOSION);
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

            NetOutgoingMessage scoreMsg = CreateNetMessage();
            scoreMsg.Write((int)PacketType.SCORE_QUERY_RESPONSE);
            scoreMsg.Write(currentPlayer.GetId());
            scoreMsg.Write(currentPlayer.Data.Score);
            currentPlayer.Statistics.WriteObject(scoreMsg);
            SendMessage(scoreMsg);
        }

        private void ReceivedNewHookMsg(NetIncomingMessage msg)
        {
            Hook h = msg.ReadObjectHook(this);
            h.ReadObject(msg);
            h.Owner = GetOpponentPlayer();
            h.SetGeometry(SceneGeometryFactory.CreateHookHead(h));

            h.PrepareLine();
            DelayedAttachToScene(h);
            SyncReceivedObject(h, msg);
        }

        private void ReceivedNewSingularityBouncingBulletMsg(NetIncomingMessage msg)
        {
            SingularityBouncingBullet s = new SingularityBouncingBullet(this, -1);
            s.ReadObject(msg);
            s.Owner = GetOpponentPlayer();
            s.SetGeometry(SceneGeometryFactory.CreateConstantColorEllipseGeometry(s));
            DelayedAttachToScene(s);
            SyncReceivedObject(s, msg);
        }

        private void ReceivedNewSingularityExplodingBulletMsg(NetIncomingMessage msg)
        {
            SingularityExplodingBullet s = new SingularityExplodingBullet(this, -1);
            s.ReadObject(msg);
            s.Owner = GetOpponentPlayer();
            s.SetGeometry(SceneGeometryFactory.CreateConstantColorEllipseGeometry(s));
            DelayedAttachToScene(s);
            SyncReceivedObject(s, msg);
        }

        private void ReceivedNewSingularityBulletMsg(NetIncomingMessage msg)
        {
            SingularityBullet s = new SingularityBullet(this, -1);
            s.ReadObject(msg);
            s.Owner = GetOpponentPlayer();
            s.SetGeometry(SceneGeometryFactory.CreateConstantColorEllipseGeometry(s));
            DelayedAttachToScene(s);
            SyncReceivedObject(s, msg);

            //SoundManager.Instance.StartPlayingOnce(SharedDef.MUSIC_SHOOT);
        }

        private void ReceivedNewSingularityMineMsg(NetIncomingMessage msg)
        {
            SingularityMine s = new SingularityMine(this, -1);
            s.ReadObject(msg);
            s.Owner = GetOpponentPlayer();
            s.SetGeometry(SceneGeometryFactory.CreateRadialGradientEllipseGeometry(s));
            DelayedAttachToScene(s);
            SyncReceivedObject(s, msg);
        }

        private void ReceivedMinorAsteroidSpawnMsg(NetIncomingMessage msg)
        {
            float speed = msg.ReadFloat();
            int radius = msg.ReadInt32();
            Vector direction = msg.ReadVector();
            Vector center = msg.ReadVector();
            int rot = msg.ReadInt32();
            int textureId = msg.ReadInt32();
            int destoryerId = msg.ReadInt32();

            MinorAsteroid a1 = SceneObjectFactory.CreateSmallAsteroid(this, direction, center, rot, textureId, radius, speed, Math.PI / 12);
            a1.Id = msg.ReadInt64();
            MinorAsteroid a2 = SceneObjectFactory.CreateSmallAsteroid(this, direction, center, rot, textureId, radius, speed, 0);
            a2.Id = msg.ReadInt64();
            MinorAsteroid a3 = SceneObjectFactory.CreateSmallAsteroid(this, direction, center, rot, textureId, radius, speed, -Math.PI / 12);
            a3.Id = msg.ReadInt64();

            long parentId = msg.ReadInt64();
            UnstableAsteroid p = GetSceneObject(parentId) as UnstableAsteroid;
            if (p == null)
                p = new UnstableAsteroid(this, parentId);
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
            StatPowerUp p = new StatPowerUp(this, -1);
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
            pl.Data.LobbyReady = msg.ReadBoolean();
            UpdateLobbyPlayers();
            CheckAllPlayersReady();
        }

        private void ReceivedAllAsteroidsMsg(NetIncomingMessage msg)
        {
            if (objects.Count > 2)
            {
                Logger.Error("Receiving All Asteroids packet but already have " + objects.Count + " objects");
                return;
            }

            int count = msg.ReadInt32();
            for (int i = 0; i < count; ++i)
            {
                if (msg.ReadInt32() != (int)PacketType.NEW_ASTEROID)
                {
                    Logger.Error("Corrupted object PacketType.SYNC_ALL_ASTEROIDS");
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
                    p.Mine = (p.Mine.NextSpecialAction() as WeaponUpgrade).GetWeapon();
                    break;
                case DeviceType.CANNON:
                    p.Canoon = (p.Canoon.NextSpecialAction() as WeaponUpgrade).GetWeapon();
                    break;
                case DeviceType.HOOK:
                    p.Hook = (p.Hook.NextSpecialAction() as WeaponUpgrade).GetWeapon();
                    break;
                case DeviceType.HEALING_KIT:
                    break;
            }
        }

        private void ReceivedFloatingTextMsg(NetIncomingMessage msg)
        {
            FloatingTextMgr.AddFloatingText(msg.ReadString(), msg.ReadVector(), msg.ReadFloat(), (FloatingTextType)msg.ReadByte(), msg.ReadFloat(), true, false);
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

        private void ReceiveModuleDamage(NetIncomingMessage msg)
        {
            Player player = GetPlayer(msg.ReadInt32());
            player.Device.GetControlOfType<IHpControl>().Hp = msg.ReadInt32();
            int damage = msg.ReadInt32();

            player.Statistics.DamageTaken += damage;
            
            HpRegenControl control = player.Device.GetControlOfType<HpRegenControl>();
            if (control != null)
                control.TakeHit();
        }

        private void ChangeMoveState(NetIncomingMessage msg)
        {
            Player player = GetPlayer(msg.ReadInt32());
            player.Device.GetControlOfType<ControlableDeviceControl>().ReceivedMovingTypeChanged(msg);
        }

        private void ReceiveModuleColorChange(NetIncomingMessage msg)
        {
            Player owner = GetPlayer(msg.ReadInt32());
            HpBarControl control = owner.Device.GetControlOfType<HpBarControl>();

            control.Bar.Color = msg.ReadColor();
        }

        private void ReceivePlayerColorChange(NetIncomingMessage msg)
        {
            Player plr = GetPlayer(msg.ReadInt32());
            if (plr != null)
            {
                plr.Data.PlayerColor = msg.ReadColor();
                UpdateLobbyPlayers();
            }
        }

        private void ReceiveObjectsHeal(NetIncomingMessage msg)
        {
            Player owner = GetPlayer(msg.ReadInt32());
            int count = msg.ReadInt32();
            int dmg = msg.ReadInt32();

            IDestroyable obj;

            for (int i = 0; i < count; i++)
            {
                obj = GetSceneObject(msg.ReadInt64()) as IDestroyable;

                if (obj != null)
                {
                    obj.TakeDamage(-dmg, owner.Device);
                    FloatingTextMgr.AddFloatingText("+" + dmg, obj.Position, FloatingTextManager.TIME_LENGTH_3, FloatingTextType.HEAL);
                }
            }
        }

        private void ReceiveObjectsDamage(NetIncomingMessage msg)
        {
            Player owner = GetPlayer(msg.ReadInt32());
            int count = msg.ReadInt32();
            int dmg = msg.ReadInt32();

            IDestroyable obj;

            for (int i = 0; i < count; i++)
            {
                obj = GetSceneObject(msg.ReadInt64()) as IDestroyable;

                if (obj != null)
                {
                    obj.TakeDamage(dmg, owner.Device);
                    FloatingTextMgr.AddFloatingText(dmg, obj.Position, FloatingTextManager.TIME_LENGTH_3, FloatingTextType.DAMAGE);
                }
            }
        }

        private void ReceiveAsteroidsDirectionChange(NetIncomingMessage msg)
        {
            int count = msg.ReadInt32();
            Asteroid ast = null;
            Vector dir;
            for (int i = 0; i < count; i++)
            {
                ast = GetSceneObject(msg.ReadInt64()) as Asteroid;
                dir = msg.ReadVector();

                if (ast != null)
                {
                    ast.Direction = dir;
                    IMovementControl control = ast.GetControlOfType<IMovementControl>();
                    if (control != null)
                        control.Speed = SharedDef.SPECTATOR_ASTEROID_THROW_SPEED;
                }
            }
        }

        private void ReceivedTournamentSettingsMsg(NetIncomingMessage msg)
        {
            players.ForEach(p => { if (!p.Data.LobbyLeader) p.Data.LobbyReady = false; });
            lastTournamentSettings = msg.ReadTournamentSettings();

            List<LobbyPlayerData> data = CreateLobbyPlayerData();

            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                LobbyUC lobby = LogicalTreeHelper.FindLogicalNode(Application.Current.MainWindow, "lobbyWindow") as LobbyUC;
                if (lobby != null)
                {
                    lobby.UpdateTournamentSettings(lastTournamentSettings);
                    lobby.UpdateShownPlayers(data);
                }
            }));
        }

        private void ReceivedSpectatorActionStarted(NetIncomingMessage msg)
        {
            Player owner = GetPlayer(msg.ReadInt32());
            String name = msg.ReadString();

            FloatingTextMgr.AddFloatingText("Spectator " + owner.Data.Name + " used: " + name, owner.Device.Position, FloatingTextManager.TIME_LENGTH_4, FloatingTextType.SYSTEM, FloatingTextManager.SIZE_MEDIUM);
        }

        private void ReceivedPlayerScoreUpdate(NetIncomingMessage msg)
        {
            currentPlayer.Data.Score = msg.ReadInt32();
        }

        public void SendNewTournamentSettings(TournamentSettings s)
        {
            NetOutgoingMessage msg = CreateNetMessage();
            msg.Write((int)PacketType.TOURNAMENT_SETTINGS);
            msg.Write(s);
            SendMessage(msg);
        }

        public void ProcessNewTournamentSettings(TournamentSettings s)
        {
            lastTournamentSettings = s;
            SendNewTournamentSettings(s);
            players.ForEach(p => { if (!p.Data.LobbyLeader) p.Data.LobbyReady = false; });
            UpdateLobbyPlayers();
        }

        private void ReceivedHookForcePullMsg(NetIncomingMessage msg)
        {
            long id = msg.ReadInt64();
            PowerHook hook = GetSceneObject(id) as PowerHook;
            if (hook == null)
            {
                Logger.Warn("Received PowerHook Force Pull but hook with id " + id + " wasnt found -> ignored");
                return;
            }

            PowerHookControl c = hook.GetControlOfType<PowerHookControl>();
            c.ShowForceFieldEffect();

            int count = msg.ReadInt32();
            for (int i = 0; i < count; ++i)
            {
                ISceneObject obj = GetSceneObject(msg.ReadInt64());
                if (obj != null)
                    c.StartPullingObject(obj);
            }
        }

        public void RequestKickPlayer(int playerId)
        {
            NetOutgoingMessage msg = CreateNetMessage();
            msg.Write((int)PacketType.PLAYER_KICK_REQUEST);
            msg.Write(playerId);
            SendMessage(msg);

            Player kicked = GetPlayer(playerId);
            if (kicked != null)
                SendChatMessage(kicked.Data.Name + " has been kicked by the leader", true);
        }
    }
}
