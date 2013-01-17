using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Orbit.Core.Client;
using System.Windows.Media;
using Orbit.Core.Helpers;
using System.Windows.Media.Effects;

namespace Orbit.Core.Scene.Entities.HeavyWeight
{
    class HeavyweightLine : Line, IHeavyWeightSceneObject
    {
        private static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public UIElement HeavyWeightGeometry { get; set; }

        public override bool Visible
        {
            get
            {
                return visible;
            }
            set
            {
                visible = value;
                if (HeavyWeightGeometry == null)
                    return;
                SceneMgr.BeginInvoke(new Action(() => HeavyWeightGeometry.Visibility = visible ? Visibility.Visible : Visibility.Hidden));
            }
        }

        public HeavyweightLine(SceneMgr mgr, long id, Vector start, Vector end, Color color, int width)
            : base(mgr, id)
        {
            SceneMgr = mgr;
            Start = start;
            End = end;

            SceneMgr.BeginInvoke(new Action(() => {
                HeavyWeightGeometry = HeavyweightGeometryFactory.CreateLineGeometry(Start, End, color, width);
                BlurEffect effect = new BlurEffect();
                effect.Radius = 6;
                effect.KernelType = KernelType.Box;
                HeavyWeightGeometry.Effect = effect;
            }));
        }

        /// <summary>
        /// prepisujte jen pokud je to nezbytne, jinak pouzijte UpdateGeometricState()!
        /// </summary>
        public override void UpdateGeometric()
        {
            System.Windows.Shapes.Line line = HeavyWeightGeometry as System.Windows.Shapes.Line;
            line.X1 = Start.X;
            line.Y1 = Start.Y;
            line.X2 = End.X;
            line.Y2 = End.Y;
        }


        public override void SetGeometry(DrawingGroup geometryElement)
        {
            Logger.Warn("Trying to set DrawingGroup geometry to HeavyweightSphere -> ignoring!");
        }
    }
}
