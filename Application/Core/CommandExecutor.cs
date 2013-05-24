using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Orbit.Core.Server.Match;
using Orbit.Gui;
using Orbit.Core.Server.Level;
using Orbit.Core.Players;

namespace Orbit.Core
{
    public enum CommandType 
    {
        START_LOCAL_TOURNAMENT,
        CONNECT_FIRST_TOURNAMENT,
        CREATE_COMPETITION_TOURNAMENT
    }

    public class CommandExecutor
    {
        private static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static Dictionary<string, CommandType> commandTypes = new Dictionary<string, CommandType>()
        {
            { "-localtournament", CommandType.START_LOCAL_TOURNAMENT },
            { "-autoconnect", CommandType.CONNECT_FIRST_TOURNAMENT },
            { "-autocreate", CommandType.CREATE_COMPETITION_TOURNAMENT },
        };

        private static CommandExecutor instance;
        public static CommandExecutor Instance
        {
            get
            {
                if (instance == null)
                    instance = new CommandExecutor();
                return instance;
            }
        }

        private static List<CommandType> commands = new List<CommandType>();

        private void AddCommand(CommandType type)
        {
            commands.Add(type);
        }

        public void ParseCommands(string[] args)
        {
            foreach (string s in args)
                if (commandTypes.ContainsKey(s))
                    AddCommand(commandTypes[s]);
        }

        public void ExecuteCommands()
        {
            foreach (CommandType c in commands)
            {
                switch (c)
                {
                    case CommandType.START_LOCAL_TOURNAMENT:
                        App.Instance.StartTournamentFinder("127.0.0.1");
                        break;
                    case CommandType.CONNECT_FIRST_TOURNAMENT:
                        TournamentFinderUC tuc = LogicalTreeHelper.FindLogicalNode(App.WindowInstance, "tournamentFinderUC") as TournamentFinderUC;
                        if (tuc != null)
                        {
                            tuc.ReceivedNewTournamentsCallback = (settings, serverAddress) =>
                            {
                                if (settings.Count > 0)
                                {
                                    App.Instance.StartTournamentGame(serverAddress, settings[0].ServerId);
                                    tuc.ReceivedNewTournamentsCallback = null;
                                }
                            };
                        }
                        break;
                    case CommandType.CREATE_COMPETITION_TOURNAMENT:
                        TournamentSettings s = new TournamentSettings();
                        s.Name = "Automatic Test Competition";
                        s.Leader = App.Instance.PlayerName;
                        s.MMType = MatchManagerType.ONLY_SCORE;
                        s.Level = GameLevel.BASIC_MAP;
                        s.RoundCount = 10;
                        s.BotCount = 0;
                        s.BotType = BotType.NONE;

                        App.Instance.StartTournamentLobby("127.0.0.1");

                        App.Instance.GetSceneMgr().Enqueue(new Action(() =>
                        {
                            App.Instance.GetSceneMgr().ProcessNewTournamentSettings(s);
                        }));

                        LobbyUC lobby = LogicalTreeHelper.FindLogicalNode(Application.Current.MainWindow, "lobbyWindow") as LobbyUC;
                        if (lobby != null)
                            lobby.UpdateTournamentSettings(s);
                        break;
                    
                }
            }
        }
    }
}
