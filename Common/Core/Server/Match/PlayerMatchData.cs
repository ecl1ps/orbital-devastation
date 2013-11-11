using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orbit.Core.Server.Match
{
    public class PlayerMatchData
    {
        public List<string> playedWith = new List<string>();
        public int WonGames { get; set; }
        public string Owner { get; set; }
        public bool IsOnline { get; set; }

        public PlayerMatchData(string owner)
        {
            Owner = owner;
            IsOnline = true;
        }
    }
}
