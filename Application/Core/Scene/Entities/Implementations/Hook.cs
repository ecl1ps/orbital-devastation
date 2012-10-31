using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Scene.Entities;
using Orbit.Core.Players;
using Orbit.Core.Scene.Controls;
using System.Windows;
using Lidgren.Network;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Windows.Media;
using Orbit.Core.Client;
using Orbit.Core.Helpers;
using Orbit.Core.Scene.Entities.Implementations;
using Orbit.Core.Scene.Controls.Implementations;
using System.Diagnostics;
using Orbit.Core.Client.GameStates;

namespace Orbit.Core.Scene.Entities.Implementations
{
    public enum HookType
    {
        HOOK_NORMAL,
        HOOK_POWER
    }

    public class Hook : SpherePoint, ISendable, IRotable, IProjectile
    {
        public Player Owner { get; set; } // neposilano
        public float Rotation { get; set; }
        public ICatchable CaughtObject { get; set; } // neposilano
        public HookType HookType { get; set; }
        public Vector RopeContactPoint
        {
            get
            {
                return Center - Direction * Radius;
            }
        }

        private Line line; // neposilano
        
        public Hook(SceneMgr mgr) : base(mgr)
        {
            HookType = HookType.HOOK_NORMAL;
        }

        public void PrepareLine()
        {
            SceneMgr.Invoke(new Action(() =>
            {
                HookControl control = GetControlOfType(typeof(HookControl)) as HookControl;
                line = new Line();
                line.Stroke = Brushes.LightSteelBlue;
                line.X1 = control.Origin.X;
                line.Y1 = control.Origin.Y;
                line.X2 = control.Origin.X;
                line.Y2 = control.Origin.Y;
                line.HorizontalAlignment = HorizontalAlignment.Left;
                line.VerticalAlignment = VerticalAlignment.Center;
                line.StrokeThickness = 2;
                line.Fill = new SolidColorBrush(Colors.Black);
            }));

            SceneMgr.AttachGraphicalObjectToScene(line);
        }

        protected override void UpdateGeometricState()
        {
            line.X2 = RopeContactPoint.X;
            line.Y2 = RopeContactPoint.Y;
        }

        public override void DoCollideWith(ICollidable other, float tpf)
        {
            if (HasCaughtObject())
                return;

            if ((other is ICatchable && !(GetControlOfType(typeof(HookControl)) as HookControl).Returning))
                CatchObject(other as ICatchable);
        }

        protected void CatchObject(ICatchable caught)
        {
            if (caught == null)
                return;

            if (!caught.Enabled)
                return;

            if (!Owner.IsCurrentPlayerOrBot())
                return;

            processScore(caught);

            Vector hitVector = caught.Position - Position;

            if (caught.Enabled)
            {
                Catch(caught, hitVector);

                NetOutgoingMessage msg = SceneMgr.CreateNetMessage();
                msg.Write((int)PacketType.HOOK_HIT);
                msg.Write(Id);
                msg.Write(caught.Id);
                msg.Write(caught.Position);
                msg.Write(hitVector);
                SceneMgr.SendMessage(msg);
            }
        }

        protected virtual void processScore(ICatchable caught)
        {
            if (caught is IContainsGold)
            {
                if (Owner.IsCurrentPlayer())
                    SceneMgr.FloatingTextMgr.AddFloatingText(ScoreDefines.HOOK_HIT, Center, FloatingTextManager.TIME_LENGTH_1,
                        FloatingTextType.SCORE);
                Owner.AddScoreAndShow(ScoreDefines.HOOK_HIT);
            }

            HookControl control = GetControlOfType(typeof(HookControl)) as HookControl;
            if (control != null && control.GetDistanceFromOriginPct() > 0.9)
            {
                SceneMgr.FloatingTextMgr.AddFloatingText(ScoreDefines.HOOK_CAUGHT_OBJECT_AFTER_90PCT_DISTANCE, Center,
                    FloatingTextManager.TIME_LENGTH_4, FloatingTextType.SCORE, FloatingTextManager.SIZE_BIG, false, true);
                Owner.AddScoreAndShow(ScoreDefines.HOOK_CAUGHT_OBJECT_AFTER_90PCT_DISTANCE);
            }
        }

        public virtual void Catch(ICatchable caught, Vector hitVector)
        {
            if (caught is IDestroyable)
                (caught as IDestroyable).TakeDamage(0, this);

            if (caught is UnstableAsteroid)
                return;

            CaughtObject = caught;
            caught.Enabled = false;

            (GetControlOfType(typeof(HookControl)) as HookControl).CaughtObject(hitVector);
        }

        public virtual void PulledCaughtObjectToBase()
        {
            proccesCaughtObject(CaughtObject);
        }

        protected virtual void proccesCaughtObject(ICatchable caught) 
        {
            if (caught != null && !caught.Dead)
            {
                if (caught is IContainsGold)
                {
                    AddGoldToOwner((caught as IContainsGold).Gold);
                    caught.DoRemoveMe();
                }
                else
                {
                    caught.Enabled = true;
                    if (caught is IMovable)
                        (caught as IMovable).Direction = new Vector(0, 100);
                }
            }

            DoRemoveMe();
            SceneMgr.RemoveGraphicalObjectFromScene(line);
        }

        protected virtual void AddGoldToOwner(int gold) 
        {
            Owner.AddGoldAndShow(gold);
        }

        public virtual Boolean HasCaughtObject()
        {
            return CaughtObject != null;
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
            IList<IControl> controls = msg.ReadControls();
            foreach (Control c in controls)
                AddControl(c);
        }

        public override bool IsOnScreen(Size screenSize)
        {
            return true;
        }
    }
}