﻿using OpenBulletCE.ViewModels;
using System;
using System.Windows.Controls;

namespace OpenBulletCE.Views.Main.Settings.OpenBullet
{
    /// <summary>
    /// Logica di interazione per General.xaml
    /// </summary>
    public partial class General : Page
    {
        public General()
        {
            InitializeComponent();
            DataContext = OB.OBSettings.General;
        }
    }
}
