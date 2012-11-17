using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orbit.Core
{
    public class GameStateManager
    {
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
                Console.WriteLine("This GameState is already in GameStateManager");
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
        public void RemoveGameState(Type type)
        {
            foreach (IGameState state in gameStates)
            {
                if (type.IsAssignableFrom(state.GetType()))
                    gameStates.Remove(state);
            }
        }

        /// <summary>
        /// pokud obsahuje vic states stejneho typu, tak nelze zarucit, ktery bude vracen
        /// </summary>
        public IGameState GetGameStateOfType(Type type)
        {
            foreach (IGameState state in gameStates)
            {
                if (type.IsAssignableFrom(state.GetType()))
                    return state;
            }
            return null;
        }

        public void Clear()
        {
            gameStates.Clear();
        }
    }
}
