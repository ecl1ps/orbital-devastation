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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Orbit.Gui.InteractivePanel;

namespace Orbit.Gui
{
    /// <summary>
    /// Interaction logic for LeftPanel.xaml
    /// </summary>
    public partial class HidingPanel : UserControl, IInteractivePanel<ActionOverview>
    {
        public HidingPanel()
        {
            InitializeComponent();
        }

        public void AddItem(UIElement elem)
        {
            Dispatcher.BeginInvoke(new Action(() => 
            {
                if (!Panel.Children.Contains(elem))
                {
                    //hmm hack protoze nefunguje stack panel
                    Panel.Children.Add(elem);
                    Canvas.SetTop(elem, Panel.Children.Count * 30);
                }
            }));
        }

        public void RemoveItem(UIElement elem)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                UIElementCollection temp = Panel.Children;
                temp.Remove(elem);

                Panel.Children.Clear();

                foreach (UIElement element in temp)
                {
                    AddItem(element);
                }
            }));

            
        }

        public void ClearAll()
        {
            foreach (UIElement element in Panel.Children)
            {
                (element as ActionOverview).RenderAction(null);
            }
        }


        public ActionOverview getItem(int i)
        {
            ActionOverview item = null;
            item = Panel.Children[i] as ActionOverview;
            
            return item;
        }
    }
}
