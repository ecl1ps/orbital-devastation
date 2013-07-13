using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Scene.Entities;
using System.Windows;
using ShaderEffectLibrary;
using Orbit.Core.Scene.Entities.Implementations;
using Orbit.Core.Weapons;
using Orbit.Core.Client.Shaders;
using Orbit.Core.Client;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Orbit.Core.Helpers
{
    public class SceneGeometryFactory
    {
        public static Dictionary<string, Texture2D> Asteroids { get; set; }
        
        public static Texture2D Base_100 { get; set; }
        public static Texture2D Base_75 { get; set; }
        public static Texture2D Base_50 { get; set; }
        public static Texture2D Base_25 { get; set; }
        public static Texture2D Base_background { get; set; }

        public static Texture2D BoxBlue_1 { get; set; }
        public static Texture2D BoxBlue_2 { get; set; }
        public static Texture2D BoxBrown { get; set; }
        public static Texture2D BoxGreen { get; set; }
        public static Texture2D BoxPurple { get; set; }

        public static Texture2D Mine { get; set; }
        public static Texture2D Hook { get; set; }

        public static Texture2D MiningModule { get; set; }

        public static Texture2D Background { get; set; }

        public static void Load(ContentManager content)
        {
            Asteroids = new Dictionary<string, Texture2D>();
            Asteroids["Asteroid_gold_1"] = content.Load<Texture2D>("images/rock/rock_gold_1");
            Asteroids["Asteroid_gold_1_burn"] = content.Load<Texture2D>("images/rock/rock_gold_1_burn");
            Asteroids["Asteroid_gold_2"] = content.Load<Texture2D>("images/rock/rock_gold_2");
            Asteroids["Asteroid_gold_2_burn"] = content.Load<Texture2D>("images/rock/rock_gold_2_burn");
            Asteroids["Asteroid_gold_3"] = content.Load<Texture2D>("images/rock/rock_gold_3");
            Asteroids["Asteroid_gold_3_burn"] = content.Load<Texture2D>("images/rock/rock_gold_3_burn");
            Asteroids["Asteroid_gold_4"] = content.Load<Texture2D>("images/rock/rock_gold_4");
            Asteroids["Asteroid_gold_4_burn"] = content.Load<Texture2D>("images/rock/rock_gold_4_burn");
            Asteroids["Asteroid_gold_5"] = content.Load<Texture2D>("images/rock/rock_gold_5");
            Asteroids["Asteroid_gold_5_burn"] = content.Load<Texture2D>("images/rock/rock_gold_5_burn");

            Asteroids["Asteroid_unstable_1"] = content.Load<Texture2D>("images/rock/rock_unstable_1");
            Asteroids["Asteroid_unstable_1_burn"] = content.Load<Texture2D>("images/rock/rock_unstable_1_burn");
            Asteroids["Asteroid_unstable_2"] = content.Load<Texture2D>("images/rock/rock_unstable_2");
            Asteroids["Asteroid_unstable_2_burn"] = content.Load<Texture2D>("images/rock/rock_unstable_2_burn");
            Asteroids["Asteroid_unstable_3"] = content.Load<Texture2D>("images/rock/rock_unstable_3");
            Asteroids["Asteroid_unstable_3_burn"] = content.Load<Texture2D>("images/rock/rock_unstable_3_burn");
            Asteroids["Asteroid_unstable_4"] = content.Load<Texture2D>("images/rock/rock_unstable_4");
            Asteroids["Asteroid_unstable_4_burn"] = content.Load<Texture2D>("images/rock/rock_unstable_4_burn");
            Asteroids["Asteroid_unstable_5"] = content.Load<Texture2D>("images/rock/rock_unstable_5");
            Asteroids["Asteroid_unstable_5_burn"] = content.Load<Texture2D>("images/rock/rock_unstable_5_burn");

            Asteroids["Asteroid_1"] = content.Load<Texture2D>("images/rock/rock_normal_1");
            Asteroids["Asteroid_1_burn"] = content.Load<Texture2D>("images/rock/rock_normal_1_burn");
            Asteroids["Asteroid_2"] = content.Load<Texture2D>("images/rock/rock_normal_2");
            Asteroids["Asteroid_2_burn"] = content.Load<Texture2D>("images/rock/rock_normal_2_burn");
            Asteroids["Asteroid_3"] = content.Load<Texture2D>("images/rock/rock_normal_3");
            Asteroids["Asteroid_3_burn"] = content.Load<Texture2D>("images/rock/rock_normal_3_burn");
            Asteroids["Asteroid_4"] = content.Load<Texture2D>("images/rock/rock_normal_4");
            Asteroids["Asteroid_4_burn"] = content.Load<Texture2D>("images/rock/rock_normal_4_burn");
            Asteroids["Asteroid_5"] = content.Load<Texture2D>("images/rock/rock_normal_5");
            Asteroids["Asteroid_5_burn"] = content.Load<Texture2D>("images/rock/rock_normal_5_burn");
            Asteroids["Asteroid_6"] = content.Load<Texture2D>("images/rock/rock_normal_6");
            Asteroids["Asteroid_6_burn"] = content.Load<Texture2D>("images/rock/rock_normal_6_burn");
            Asteroids["Asteroid_7"] = content.Load<Texture2D>("images/rock/rock_normal_7");
            Asteroids["Asteroid_7_burn"] = content.Load<Texture2D>("images/rock/rock_normal_7_burn");
            Asteroids["Asteroid_8"] = content.Load<Texture2D>("images/rock/rock_normal_8");
            Asteroids["Asteroid_8_burn"] = content.Load<Texture2D>("images/rock/rock_normal_8_burn");
            Asteroids["Asteroid_9"] = content.Load<Texture2D>("images/rock/rock_normal_9");
            Asteroids["Asteroid_9_burn"] = content.Load<Texture2D>("images/rock/rock_normal_9_burn");
            Asteroids["Asteroid_10"] = content.Load<Texture2D>("images/rock/rock_normal_10");
            Asteroids["Asteroid_10_burn"] = content.Load<Texture2D>("images/rock/rock_normal_10_burn");
            Asteroids["Asteroid_11"] = content.Load<Texture2D>("images/rock/rock_normal_11");
            Asteroids["Asteroid_11_burn"] = content.Load<Texture2D>("images/rock/rock_normal_11_burn");
            Asteroids["Asteroid_12"] = content.Load<Texture2D>("images/rock/rock_normal_12");
            Asteroids["Asteroid_12_burn"] = content.Load<Texture2D>("images/rock/rock_normal_12_burn");
            Asteroids["Asteroid_13"] = content.Load<Texture2D>("images/rock/rock_normal_13");
            Asteroids["Asteroid_13_burn"] = content.Load<Texture2D>("images/rock/rock_normal_13_burn");
            Asteroids["Asteroid_14"] = content.Load<Texture2D>("images/rock/rock_normal_14");
            Asteroids["Asteroid_14_burn"] = content.Load<Texture2D>("images/rock/rock_normal_14_burn");
            Asteroids["Asteroid_15"] = content.Load<Texture2D>("images/rock/rock_normal_15");
            Asteroids["Asteroid_15_burn"] = content.Load<Texture2D>("images/rock/rock_normal_15_burn");
            Asteroids["Asteroid_16"] = content.Load<Texture2D>("images/rock/rock_normal_16");
            Asteroids["Asteroid_16_burn"] = content.Load<Texture2D>("images/rock/rock_normal_16_burn");
            Asteroids["Asteroid_17"] = content.Load<Texture2D>("images/rock/rock_normal_17");
            Asteroids["Asteroid_17_burn"] = content.Load<Texture2D>("images/rock/rock_normal_17_burn");

            BoxBlue_1 = content.Load<Texture2D>("images/box/box_blue1");
            BoxBlue_2 = content.Load<Texture2D>("images/box/box_blue2");
            BoxBrown = content.Load<Texture2D>("images/box/box_brown");
            BoxGreen = content.Load<Texture2D>("images/box/box_green");
            BoxPurple = content.Load<Texture2D>("images/box/box_purple");

            Mine = content.Load<Texture2D>("images/projectiles/mine");
            Hook = content.Load<Texture2D>("images/projectiles/hook");

            MiningModule = content.Load<Texture2D>("images/mining-module/module");

            Base_100 = content.Load<Texture2D>("images/base/base_bw_shaded");
            Base_75 = content.Load<Texture2D>("images/base/base_bw_shaded_75");
            Base_50 = content.Load<Texture2D>("images/base/base_bw_shaded_50");
            Base_25 = content.Load<Texture2D>("images/base/base_bw_shaded_25");
            Base_background = content.Load<Texture2D>("images/base/base_background_city");

            Background = content.Load<Texture2D>("images/background/pozadi");
        }

        public static Texture2D GetAsteroidTexture(Asteroid a)
        {
            if (a.AsteroidType == AsteroidType.UNSTABLE)
                return Asteroids["Asteroid_unstable_" + a.TextureId];
            else if (a.AsteroidType == AsteroidType.GOLDEN)
                return Asteroids["Asteroid_gold_" + a.TextureId];
            else
                return Asteroids["Asteroid_" + a.TextureId];
        }

        public static Texture2D GetAsteroidOverlayTexture(Asteroid a)
        {
            if (a.AsteroidType == AsteroidType.UNSTABLE)
                return Asteroids["Asteroid_unstable_" + a.TextureId + "_burn"];
            else if (a.AsteroidType == AsteroidType.GOLDEN)
                return Asteroids["Asteroid_gold_" + a.TextureId + "_burn"];
            else
                return Asteroids["Asteroid_" + a.TextureId + "_burn"];
        }

        public static Texture2D GetPowerupTexture(StatPowerUp powerup)
        {

            if (powerup.PowerUpType == DeviceType.MINE)
                return BoxBlue_1;
            else if (powerup.PowerUpType == DeviceType.CANNON)
                return BoxBrown;
            else if (powerup.PowerUpType == DeviceType.HOOK)
                return BoxBrown;
            else
                return BoxGreen;

        }
    }
}
