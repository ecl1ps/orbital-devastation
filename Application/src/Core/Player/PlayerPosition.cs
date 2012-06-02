using System;
using System.Windows;
using Orbit.Core.Scene.Entities;
using Orbit.Core.Scene;

namespace Orbit.Core.Players
{

	public enum PlayerPosition {
        LEFT,
        RIGHT,
        INVALID
	}

    public class PlayerPositionProvider 
    {
        public static Vector getVectorPosition(PlayerPosition position)
        {
            Vector vector = new Vector(SceneMgr.GetInstance().ViewPortSizeOriginal.Width, SceneMgr.GetInstance().ViewPortSizeOriginal.Height * 0.85);

            switch (position)
            {
                case PlayerPosition.LEFT:
                    vector.X *= 0.1;
                    break;
                case PlayerPosition.RIGHT:
                    vector.X *= 0.6;
                    break;
                default:
                    return new Vector();
            }

            return vector;
        }
    }
}
