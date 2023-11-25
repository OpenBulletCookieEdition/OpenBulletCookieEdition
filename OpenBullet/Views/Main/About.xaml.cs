﻿using System.Diagnostics;
using System.Windows.Controls;

namespace OpenBulletCE.Views.Main
{
    /// <summary>
    /// Logica di interazione per About.xaml
    /// </summary>
    public partial class About : Page
    {
        public About()
        {
            InitializeComponent();
        }

        private void repoButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Process.Start("https://github.com/");
        }

        private void docuButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Process.Start("https://openbullet.github.io/");
        }
    }
}
