﻿using System;
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

namespace Orbit.Gui
{
    /// <summary>
    /// Interaction logic for LobbyPlayer.xaml
    /// </summary>
    public partial class LobbyPlayer : UserControl
    {
        public int PlayerId { get; set; }

        public LobbyPlayer(LobbyPlayerData data)
        {
            InitializeComponent();
            PlayerId = data.Id;
            lblName.Content = data.Name;
            lblScore.Content = data.Score;
        }
    }

    public class LobbyPlayerData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Score { get; set; }

        public LobbyPlayerData(int id, string name, int score)
        {
            Id = id;
            Name = name;
            Score = score;
        }
    }
}