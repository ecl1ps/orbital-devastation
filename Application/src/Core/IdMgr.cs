using System;
using Orbit.Core.Scene;

namespace Orbit.Core
{
    class IdMgr
    {
        private static IdMgr idMgr;
        private long maxId;

        private IdMgr()
        {
            maxId = 0;
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

        public static long GetNewId()
        {
            if (SceneMgr.GetInstance().GameType == Gametype.CLIENT_GAME)
                return GetInstance().GetIdNew() | (1L << 32);

            return GetInstance().GetIdNew();
        }
    }
}
