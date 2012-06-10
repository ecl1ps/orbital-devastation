using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Scene.Entities;
using Orbit.Core.Scene;
using Orbit.Core;
using Orbit.Core.Scene.Controls;

namespace Orbit.src.Core.Scene.Entities.Implementations
{
    class UnstableAsteroid : Asteroid
    {

        private void spawnSmallMeteors(int radius)
        {
            radius /= 3;
            if(radius < 10)
                return;

            createSmallAsteroid(radius, Math.PI / 12);
            createSmallAsteroid(radius, 0);
            createSmallAsteroid(radius, -Math.PI / 12);
        }

        private void createSmallAsteroid(int radius, double rotation)
        {
            Asteroid asteroid = new Asteroid();
            asteroid.AsteroidType = AsteroidType.NORMAL;
            asteroid.Rotation = Rotation;
            asteroid.Direction = Direction.Rotate(rotation);
            asteroid.Radius = radius;
            copyControls(asteroid);
            asteroid.SetGeometry(SceneGeometryFactory.CreateAsteroidImage(asteroid));
        }

        private void copyControls(Asteroid asteroid)
        {
            IList<IControl> controls = GetControlsCopy();

            foreach (IControl control in controls) 
            {
                asteroid.AddControl(control as Control);
            }
        }
    }
}
