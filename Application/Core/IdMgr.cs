using System;
using Orbit.Core.Scene;

namespace Orbit.Core
{
    public class IdMgr
    {
        private static IdMgr idMgr;
        private long maxId;
        private int playerId;
        private int serverId;

        private IdMgr()
        {
            maxId = 0;
            playerId = 0;
            serverId = 0;
        }

        private static IdMgr GetInstance()
        {
            if (idMgr == null)
                idMgr = new IdMgr();
            return idMgr;
        }

        private long GetIdNew()
        {
            if (maxId + 1 == long.MaxValue)
                maxId = 0;

            return ++maxId;
        }

        public static long GetNewId(int highOwnerId)
        {
            return GetInstance().GetIdNew() | ((long)highOwnerId << 32);
        }

        public static int GetNewPlayerId()
        {
            if (GetInstance().playerId + 1 == int.MaxValue)
                GetInstance().playerId = 0;

            return ++(GetInstance().playerId);
        }

        public static int GetLowId(long fullId)
        {
            return (int)(fullId & 0xFFFFFFFF);
        }

        public static int GetHighId(long fullId)
        {
            return (int)(fullId & 0xFFFFFFFFL << 32);
        }

        public static int GetNewServerId()
        {
            if (GetInstance().serverId + 1 == int.MaxValue)
                GetInstance().serverId = 0;

            return ++(GetInstance().serverId);
        }
    }
}
