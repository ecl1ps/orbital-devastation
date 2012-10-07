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
using Orbit.Core;

namespace Orbit.Gui
{
    /// <summary>
    /// Interaction logic for KeyBindingsOptions.xaml
    /// </summary>
    public partial class KeyBindingsOptions : UserControl
    {
        private bool waitingForInput = false;
        PropertyKey keyToFill;
        TextBlock labelToFill;

        public KeyBindingsOptions()
        {
            InitializeComponent();
            LoadKeys();
        }
        
        private void LoadKeys()
        {
            Action1.Text = parseKey(GameProperties.Props.Get(PropertyKey.PLAYER_ACTION_1)).ToString();
            Action2.Text = parseKey(GameProperties.Props.Get(PropertyKey.PLAYER_ACTION_2)).ToString();
            Action3.Text = parseKey(GameProperties.Props.Get(PropertyKey.PLAYER_ACTION_3)).ToString();
            Action4.Text = parseKey(GameProperties.Props.Get(PropertyKey.PLAYER_ACTION_4)).ToString();
            Action5.Text = parseKey(GameProperties.Props.Get(PropertyKey.PLAYER_ACTION_5)).ToString();

            MoveTop.Text = parseKey(GameProperties.Props.Get(PropertyKey.PLAYER_ACTION_MOVE_TOP)).ToString();
            MoveBot.Text = parseKey(GameProperties.Props.Get(PropertyKey.PLAYER_ACTION_MOVE_BOT)).ToString();
            MoveLeft.Text = parseKey(GameProperties.Props.Get(PropertyKey.PLAYER_ACTION_MOVE_LEFT)).ToString();
            MoveRight.Text = parseKey(GameProperties.Props.Get(PropertyKey.PLAYER_ACTION_MOVE_RIGHT)).ToString();

            keyBindingsMenu.Focusable = true;
        }

        private Key parseKey(String key)
        {
            return (Key) Int32.Parse(key);
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

        private void Grid_KeyDown(object sender, KeyEventArgs e)
        {
            if (waitingForInput)
                ProccessInput(e.Key);
        }

        private void ProccessInput(Key key)
        {
            labelToFill.Text = key.ToString();

            GameProperties.Props.SetAndSave(keyToFill, ((int) key));
            waitingForInput = false;
        }

        private void WaitForInput(TextBlock label, PropertyKey key)
        {
            if (waitingForInput)
                labelToFill.Text = parseKey(GameProperties.Props.Get(keyToFill)).ToString();

            label.Text = "Press any key";
            labelToFill = label;
            keyToFill = key;

            waitingForInput = true;
            keyBindingsMenu.Focus();
        }
    }
}
