using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using System.Windows.Controls;
using Orbit.Core.Client;
using Orbit.Core.Helpers;
using Orbit.Core.Scene.Entities.Implementations;

namespace Orbit.Core.Scene.Entities
{
    public abstract class HeavyweightSphere : Sphere, IHeavyWeightSceneObject
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

        public HeavyweightSphere(SceneMgr mgr, long id)
            : base(mgr, id)
        {
        }

        /// <summary>
        /// prepisujte jen pokud je to nezbytne, jinak pouzijte UpdateGeometricState()!
        /// </summary>
        public override void UpdateGeometric()
        {
            (HeavyWeightGeometry.RenderTransform as TransformGroup).Children.Clear();
            (HeavyWeightGeometry.RenderTransform as TransformGroup).Children.Add(new RotateTransform(Rotation, Radius, Radius));
            (HeavyWeightGeometry.RenderTransform as TransformGroup).Children.Add(new TranslateTransform(Position.X, Position.Y));

            UpdateGeometricState();
        }


        public override void SetGeometry(DrawingGroup geometryElement)
        {
            Logger.Warn("Trying to set DrawingGroup geometry to HeavyweightSphere -> ignoring!");
        }
    }    
}
