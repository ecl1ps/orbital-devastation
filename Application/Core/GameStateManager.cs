using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orbit.Core
{
    public class GameStateManager
    {
        private static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private List<IGameState> gameStates = new List<IGameState>();

        public void Update(float tpf)
        {
            gameStates.ForEach(a => a.Update(tpf));
        }

        public void AddGameState(IGameState state)
        {
            if (state == null)
                return;

            if (gameStates.Contains(state))
            {
                Logger.Warn("This GameState (" + state + ") is already in GameStateManager");
                return;
            }

            gameStates.Add(state);
        }

        public void RemoveGameState(IGameState state)
        {
            gameStates.Remove(state);
        }

        /// <summary>
        /// pokud obsahuje vic states stejneho typu, tak budou odstraneny vsechny
        /// </summary>
        public void RemoveGameStateOfType<T>()
        {
            for (int i = 0; i < gameStates.Count; ++i)
            {
                if (typeof(T).IsAssignableFrom(gameStates[i].GetType()))
                    gameStates.RemoveAt(i);
            }
        }

        /// <summary>
        /// pokud obsahuje vic states stejneho typu, tak nelze zarucit, ktery bude vracen
        /// </summary>
        public T GetGameStateOfType<T>()
        {
            for (int i = 0; i < gameStates.Count; ++i)
            {
                if (typeof(T).IsAssignableFrom(gameStates[i].GetType()))
                    return (T)gameStates[i];
            }
            return default(T);
        }

        public void Clear()
        {
            gameStates.Clear();
        }
    }
}
