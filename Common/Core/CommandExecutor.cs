using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Orbit.Core.Server.Match;
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
        }
    }
}
