using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Orbit.Gui
{
    /// <summary>
    /// Interaction logic for ArrowCast.xaml
    /// </summary>
    public partial class ArrowCast : UserControl
    {

        private Storyboard animation;

        public ArrowCast()
        {
            InitializeComponent();
            InitAnimation();
        }

        public void InitAnimation()
        {
            TranslateTransform transform = new TranslateTransform(0, 10);

            DoubleAnimation anim = new DoubleAnimation();
            anim.From = 0;
            anim.To = 25;
            anim.Duration = TimeSpan.FromSeconds(0.75);
            anim.RepeatBehavior = RepeatBehavior.Forever;
            anim.AutoReverse = false;

            RegisterName(Wrapper.Name, Wrapper);

            animation = new Storyboard();
            animation.Children.Add(anim);
            Storyboard.SetTargetName(anim, Wrapper.Name);
            Storyboard.SetTargetProperty(anim, new PropertyPath(Canvas.TopProperty));

        }

        public void StartAnimation()
        {
            animation.Begin(this);
        }

        public void EndAnimation()
        {
            animation.Stop();
        }
    }
}
