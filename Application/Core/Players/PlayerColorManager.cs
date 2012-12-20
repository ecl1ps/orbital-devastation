using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Windows.Media;

namespace Orbit.Core.Players
{
    public class PlayerColorManager
    {
        private static PlayerColorManager instance;
        private Dictionary<PlayerColorSet, IList<Color>> allColors;
        private IList<Color> playerColors;

        private static PlayerColorManager GetInstance()
        {
            if (instance == null)
                instance = new PlayerColorManager();

            return instance;
        }

        private PlayerColorManager()
        {
            allColors = new Dictionary<PlayerColorSet, IList<Color>>();

            IList<Color> colors = new List<Color>();
            colors.Add(Colors.CornflowerBlue);
            colors.Add(Colors.Tan);
            colors.Add(Colors.PaleVioletRed);
            colors.Add(Colors.PaleTurquoise);
            colors.Add(Colors.LemonChiffon);
            allColors.Add(PlayerColorSet.BASIC, colors);

            colors = new List<Color>();
            colors.Add(Colors.Pink);
            allColors.Add(PlayerColorSet.TUTORIAL_FINISHED, colors);

            colors = new List<Color>();
            colors.Add(Colors.Blue);
            colors.Add(Colors.Red);
            colors.Add(Colors.Gray);
            allColors.Add(PlayerColorSet.REWARD_1, colors);

            colors = new List<Color>();
            colors.Add(Colors.Goldenrod);
            colors.Add(Colors.Green);
            colors.Add(Colors.Orange);
            colors.Add(Colors.DarkOrchid);
            allColors.Add(PlayerColorSet.REWARD_2, colors);

            colors = new List<Color>();
            foreach (System.Reflection.PropertyInfo prop in typeof(Colors).GetProperties())
            {
                if (prop.PropertyType.FullName == "System.Windows.Media.Color")
                    colors.Add((Color)prop.GetGetMethod().Invoke(null, null));
            }
            allColors.Add(PlayerColorSet.ALL, colors);

            LoadPlayerColors();
        }

        public static void RefreshPlayerColors()
        {
            GetInstance().LoadPlayerColors();
        }

        private void LoadPlayerColors()
        {
            playerColors = new List<Color>();
            int plrMask = int.Parse(GameProperties.Props.Get(PropertyKey.AVAILABLE_COLORS));
            int mask = 1;
            do
            {
                if ((plrMask & mask) != 0)
                    AddColors((PlayerColorSet)mask);
                mask *= 2;
            } while (mask < (int)PlayerColorSet.END);
        }

        private void AddColors(PlayerColorSet set)
        {
            IList<Color> colors = new List<Color>();
            allColors.TryGetValue((PlayerColorSet)set, out colors);
            foreach (Color c in colors)
                playerColors.Add(c);
        }

        private void AddColorSetReal(PlayerColorSet set)
        {
            AddColors(set);
            int plrMask = int.Parse(GameProperties.Props.Get(PropertyKey.AVAILABLE_COLORS));
            plrMask |= (int)set;
            GameProperties.Props.SetAndSave(PropertyKey.AVAILABLE_COLORS, plrMask);
        }

        public static IList<Color> GetAvailableColors()
        {
            return GetInstance().playerColors;
        }

        public static void AddColorSet(PlayerColorSet set)
        {
            GetInstance().AddColorSetReal(set);
        }
    }

    public enum PlayerColorSet
    {
        BASIC = 1,
        TUTORIAL_FINISHED = 2,
        REWARD_1 = 4,
        REWARD_2 = 8,
        ALL = 16,

        END = 32
    }
}
