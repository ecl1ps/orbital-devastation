using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Scene.Entities;
using Orbit.Core.Players;
using Orbit.Core.Scene.Controls;
using System.Windows;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using Orbit.Core.Client;
using Orbit.Core.Helpers;
using Orbit.Core.Scene.Entities.Implementations;
using Orbit.Core.Scene.Controls.Implementations;
using Orbit.Core.Client.GameStates;
using Orbit.Core.Scene.Controls.Collisions.Implementations;
using Orbit.Core.Scene.CollisionShapes;

namespace Orbit.Core.Scene.Entities.Implementations
{
    public enum HookType
    {
        HOOK_NORMAL,
        HOOK_POWER
    }

    public class Hook : TexturedSphere, ISendable, IProjectile
    {
        public Player Owner { get; set; } // neposilano
        public HookType HookType { get; set; }
        public Vector2 RopeContactPoint
        {
            get
            {
                return Center - Direction * Radius;
            }
        }

        public Hook(SceneMgr mgr, long id)
            : base(mgr, id)
        {
            HookType = HookType.HOOK_NORMAL;
        }

        public override void UpdateGeometric(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch)
        {
            base.UpdateGeometric(spriteBatch);

            HookControl control = GetControlOfType<HookControl>();
            //spriteBatch.DrawLine(control.Origin, Position, Color.LightSteelBlue, 2);
        }

        public void WriteObject(NetOutgoingMessage msg)
        {
            msg.Write((int)PacketType.NEW_HOOK);
            msg.WriteObjectHook(this);
            msg.WriteControls(GetControlsCopy());
        }

        public void ReadObject(NetIncomingMessage msg)
        {
            msg.ReadObjectHook(this);

            SphereCollisionShape cs = new SphereCollisionShape();
            cs.Radius = Radius / 2;
            cs.Center = Center;
            CollisionShape = cs;

            IList<IControl> controls = msg.ReadControls();
            foreach (Control c in controls)
                AddControl(c);
        }

        public override bool IsOnScreen(Rectangle screenSize)
        {
            return true;
        }
    }
}