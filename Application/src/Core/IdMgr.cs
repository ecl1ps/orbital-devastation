using System;
using Orbit.Core.Scene;

namespace Orbit.Core
{
    class IdMgr
    {
        private static IdMgr idMgr;
        private long maxId;
        private int playerId;

        private IdMgr()
        {
            maxId = 0;
            playerId = 0;
        }

        private static IdMgr GetInstance()
        {
            if (idMgr == null)
                idMgr = new IdMgr();
            return idMgr;
        }

        private long GetIdNew()
        {
            return ++maxId;
        }

        public static long GetNewId(long highOwnerId)
        {
            return GetInstance().GetIdNew() | (highOwnerId << 32);
        }

        public static int GetNewPlayerId()
        {
            return ++(GetInstance().playerId);
        }
    }
}
