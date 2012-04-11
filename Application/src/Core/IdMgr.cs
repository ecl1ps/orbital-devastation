using System;

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

        public static IdMgr GetInstance()
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
            return GetInstance().GetIdNew();
        }
    }
}
