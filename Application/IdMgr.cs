using System;

namespace Orbit
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

        public long GetNewId()
        {
            return ++maxId;
        }
    }
}
