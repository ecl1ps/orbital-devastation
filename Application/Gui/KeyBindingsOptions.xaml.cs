using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using Microsoft.Xna.Framework;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Orbit.Core;

namespace Orbit.Gui
{
    public class KeyLabel
    {
        public TextBlock Label { get; set; }
        public PropertyKey Key { get; set; }

        public KeyLabel(PropertyKey key, TextBlock label)
        {
            Key = key;
            Label = label;
        }
    }

    /// <summary>
    /// Interaction logic for KeyBindingsOptions.xaml
    /// </summary>
    public partial class KeyBindingsOptions : UserControl
    {
        private bool waitingForInput = false;
        private KeyLabel toFill;

        private List<KeyLabel> bindings = new List<KeyLabel>();

        public KeyBindingsOptions()
        {
            InitializeComponent();

            bindings.Add(new KeyLabel(PropertyKey.PLAYER_ACTION_1, Action1));
            bindings.Add(new KeyLabel(PropertyKey.PLAYER_ACTION_2, Action2));
            bindings.Add(new KeyLabel(PropertyKey.PLAYER_ACTION_3, Action3));
            bindings.Add(new KeyLabel(PropertyKey.PLAYER_ACTION_4, Action4));
            bindings.Add(new KeyLabel(PropertyKey.PLAYER_ACTION_5, Action5));
            bindings.Add(new KeyLabel(PropertyKey.PLAYER_ACTION_MOVE_BOT, MoveBot));
            bindings.Add(new KeyLabel(PropertyKey.PLAYER_ACTION_MOVE_LEFT, MoveLeft));
            bindings.Add(new KeyLabel(PropertyKey.PLAYER_ACTION_MOVE_RIGHT, MoveRight));
            bindings.Add(new KeyLabel(PropertyKey.PLAYER_ACTION_MOVE_TOP, MoveTop));
            bindings.Add(new KeyLabel(PropertyKey.PLAYER_SHOW_ACTIONS, ShowActions));
            bindings.Add(new KeyLabel(PropertyKey.PLAYER_SHOW_PROTECTING, ShowProtecting));
            LoadKeys();
        }
        
        private void LoadKeys()
        {
            foreach (KeyLabel item in bindings)
                item.Label.Text = ParseString(GameProperties.Props.Get(item.Key)).ToString(Strings.Culture);
            
            keyBindingsMenu.Focusable = true;
        }

        private Key ParseKey(String key)
        {
            return (Key)Int32.Parse(key, CultureInfo.InvariantCulture);
        }

        private String ParseString(String key)
        {
            String result = ParseKey(key).ToString();

            if (result.Equals("D0") || result.Equals("D1") || result.Equals("D2") || result.Equals("D3") || result.Equals("D4") || result.Equals("D5") 
                || result.Equals("D6") || result.Equals("D7") || result.Equals("D8") || result.Equals("D9")) {

                    result = result.Substring(1);
            }

            return result;
        }

        private void Action1_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            WaitForInput(sender as TextBlock, PropertyKey.PLAYER_ACTION_1);
        }

        private void Action2_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            WaitForInput(sender as TextBlock, PropertyKey.PLAYER_ACTION_2);
        }

        private void Action3_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            WaitForInput(sender as TextBlock, PropertyKey.PLAYER_ACTION_3);
        }

        private void Action4_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            WaitForInput(sender as TextBlock, PropertyKey.PLAYER_ACTION_4);
        }

        private void Action5_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            WaitForInput(sender as TextBlock, PropertyKey.PLAYER_ACTION_5);
        }

        private void MoveTop_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            WaitForInput(sender as TextBlock, PropertyKey.PLAYER_ACTION_MOVE_TOP);
        }

        private void MoveBot_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            WaitForInput(sender as TextBlock, PropertyKey.PLAYER_ACTION_MOVE_BOT);
        }

        private void MoveLeft_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            WaitForInput(sender as TextBlock, PropertyKey.PLAYER_ACTION_MOVE_LEFT);
        }

        private void MoveRight_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            WaitForInput(sender as TextBlock, PropertyKey.PLAYER_ACTION_MOVE_RIGHT);
        }

        private void ShowActions_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            WaitForInput(sender as TextBlock, PropertyKey.PLAYER_SHOW_ACTIONS);
        }

        private void ShowProtecting_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            WaitForInput(sender as TextBlock, PropertyKey.PLAYER_SHOW_PROTECTING);
        }

        private void Grid_KeyDown(object sender, KeyEventArgs e)
        {
            if (waitingForInput)
                ProccessInput(e.Key);
        }

        private void ProccessInput(Key key)
        {
            if (key == Key.Escape)
                return;

            int k = (int)key;
            toFill.Label.Text = ParseString(k.ToString(Strings.Culture));

            GameProperties.Props.SetAndSave(toFill.Key, (int) key);
            waitingForInput = false;

            foreach (KeyLabel actual in bindings) {
                if (toFill.Key != actual.Key
                        && k == int.Parse(GameProperties.Props.Get(actual.Key), CultureInfo.InvariantCulture))
                {
                    GameProperties.Props.SetAndSave(actual.Key, (int) Key.None);
                    actual.Label.Text = ParseString(GameProperties.Props.Get(actual.Key));
                }
            }
        }

        private void WaitForInput(TextBlock label, PropertyKey key)
        {
            if (waitingForInput)
                toFill.Label.Text = ParseString(GameProperties.Props.Get(toFill.Key)).ToString(Strings.Culture);

            label.Text = Strings.ui_press_key;

            toFill = new KeyLabel(key, label);

            waitingForInput = true;
            keyBindingsMenu.Focus();
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            App.WindowInstance.ShowOptionsMenu();
        }
    }
}
