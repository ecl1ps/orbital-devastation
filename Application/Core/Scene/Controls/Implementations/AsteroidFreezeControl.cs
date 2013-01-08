using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Client.Shaders;
using Orbit.Core.Scene.Entities.Implementations;
using Orbit.Core.Helpers;
using System.Windows;

namespace Orbit.Core.Scene.Controls.Implementations
{
    public class AsteroidFreezeControl : TemporaryControlDisableControl
    {
        protected override void InitControl(Entities.ISceneObject me)
        {
            base.InitControl(me);
            me.SceneMgr.BeginInvoke(new Action(() =>
            {
                me.GetGeometry().Opacity = 0.5;
            }));
        }

        public override void OnRemove()
        {
            base.OnRemove();
            me.SceneMgr.BeginInvoke(new Action(() =>
            {
                me.GetGeometry().Opacity = 1;

                Random rand = me.SceneMgr.GetRandomGenerator();

                CreateShard(new Vector(0, -1), rand.Next(5), rand.NextDouble(), rand.NextDouble());
                CreateShard(new Vector(1, 0), rand.Next(5), rand.NextDouble(), rand.NextDouble());
                CreateShard(new Vector(-1, 0), rand.Next(5), rand.NextDouble(), rand.NextDouble());

            }));
        }

        private void CreateShard(Vector dir, int text, double rotation, double angle)
        {
            IceShard s = SceneObjectFactory.CreateIceShard(me.SceneMgr, me.Position, new System.Windows.Size(20, 20), text + 1);
            s.Direction = dir.Rotate(angle * (Math.PI / 4));

            LinearRotationControl rc = new LinearRotationControl();
            rc.RotationSpeed = (float) (Math.PI / 8 * rotation);
            s.AddControl(rc);

            LinearMovementControl mc = new LinearMovementControl();
            mc.Speed = 20;
            s.AddControl(mc);

            s.AddControl(new LimitedLifeControl(4));

            me.SceneMgr.DelayedAttachToScene(s);
        }
    }
}
