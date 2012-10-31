using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orbit.Core.Client.GameStates
{
    public class TimedGameWindowClose : IGameState
    {
        const float CLOSE_WINDOW_TIMER = 3;
        private float elapsedTime = 0;
        private SceneMgr sceneMgr;
        private GameEnd endType;

        public TimedGameWindowClose(SceneMgr sceneMgr, GameEnd endType)
        {
            this.sceneMgr = sceneMgr;
            this.endType = endType;
        }

        public void Update(float tpf)
        {
            elapsedTime += tpf;
            if (elapsedTime >= CLOSE_WINDOW_TIMER)
            {
                sceneMgr.CloseGameWindowAndCleanup(endType);
                elapsedTime = -100;
            }
        }
    }
}
