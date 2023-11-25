﻿using System;
using System.Collections;
using OpenBulletCE.Repositories;
using OpenBulletCE.Views.Main.Configs;
using RuriLib;
using RuriLib.Functions.Files;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using RuriLib.Enums;

namespace OpenBulletCE
{
    /// <summary>
    /// Logica di interazione per DialogNewConfig.xaml
    /// </summary>
    public partial class DialogNewConfig : Page
    {
        public object Caller { get; set; }

        public DialogNewConfig(object caller)
        {
            InitializeComponent();
            Caller = caller;
            authorTextbox.Text = OB.OBSettings.General.DefaultAuthor;
            nameTextbox.Focus();

            categoryCombobox.Items.Add(ConfigRepository.defaultCategory);
            typeCombobox.Items.Add(ConfigType.CookieEdition);
            typeCombobox.Items.Add(ConfigType.Default);

            IEnumerable categories = OB.ConfigManager.ConfigsCollection
                .Select(c => c.Category)
                .Where(category => category != ConfigRepository.defaultCategory)
                .Distinct();

            foreach (var category in categories)
                categoryCombobox.Items.Add(category);

            categoryCombobox.SelectedIndex = 0;
        }

        private void acceptButton_Click(object sender, RoutedEventArgs e)
        {
            if (Caller.GetType() == typeof(ConfigManager))
            {
                // Check if name is ok
                if (nameTextbox.Text.Trim() == string.Empty) { MessageBox.Show("The name cannot be blank"); return; }
                else if (nameTextbox.Text != Files.MakeValidFileName(nameTextbox.Text)) { MessageBox.Show("The name contains invalid characters"); return; }

                // Check if category is ok
                if (string.IsNullOrWhiteSpace(categoryCombobox.Text)) categoryCombobox.Text = ConfigRepository.defaultCategory;
                else if (categoryCombobox.Text != Files.MakeValidFileName(categoryCombobox.Text)) { MessageBox.Show("The category contains invalid characters"); return; }

                // Check if Type is ok
                if(string.IsNullOrWhiteSpace(typeCombobox.Text)) typeCombobox.Text = ConfigType.Default.ToString();
                else if(typeCombobox.Text != ConfigType.Default.ToString() && typeCombobox.Text != ConfigType.CookieEdition.ToString()) { MessageBox.Show("The Type contains invalid characters"); return; }

                try
                {
                    switch (typeCombobox.Text)
                    {
                        case "Default":
                            ((ConfigManager)Caller).CreateConfig(nameTextbox.Text, ConfigType.Default, categoryCombobox.Text, authorTextbox.Text);
                            break;
                        case "CookieEdition":
                            ((ConfigManager)Caller).CreateConfig(nameTextbox.Text, ConfigType.CookieEdition, categoryCombobox.Text, authorTextbox.Text);
                            break;
                    }
                    
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            ((MainDialog)Parent).Close();
        }

        private void textbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                acceptButton_Click(this, new RoutedEventArgs());
        }

        private void typeCombobox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                acceptButton_Click(this, new RoutedEventArgs());
        }

    }
}
