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
using System.Windows.Shapes;

namespace Cars_Inventory
{
    /// <summary>
    /// Interaction logic for WindowProductGroup_GroupRelated.xaml
    /// </summary>
    public partial class WindowProductGroup_GroupRelated : Window
    {
        private int Mode = 1; // Default to Rename Mode
        private int _IDItemGroup = 0;
        public WindowProductGroup_GroupRelated(int type, int IDItemGroup = 0, string ItemGroupOldName = "")
        {
            InitializeComponent();
            try
            {
                switch (type)
                {
                    case 2: // new
                        this.Title = "New Item Group";
                        labelOldName.Visibility = Visibility.Hidden;
                        textBoxOldName.Visibility = Visibility.Hidden;
                        labelNewGroup.Visibility = Visibility.Visible;
                        break;
                    case 1: // rename
                        this.Title = "Rename Item Group";
                        _IDItemGroup = IDItemGroup;
                        textBoxOldName.Text = ItemGroupOldName;
                        labelNewGroup.Visibility = Visibility.Hidden;
                        break;
                    default: // usually no required but for extra protection
                        throw new Exception("This is something wrong");
                }
                Mode = type;
            }
            catch (Exception ex)
            {
                using (WindowDisplayMessage wdm = new WindowDisplayMessage(ex.Message, 3))
                {
                    wdm.ShowDialog();
                    wdm.Dispose();
                    wdm.Owner = this;
                }
                this.Close();
            } // Wont accept other mode
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void buttonOK_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (textBoxNewName.Text.Length.Equals(0)) { throw new Exception("New name cannot be empty"); }
                if (Mode.Equals(1) && textBoxNewName.Text.Equals(textBoxOldName.Text)) { throw new Exception("New name cannot same like old name"); }
                switch (Mode)
                {
                    case 1: // rename
                        PublicShare.UpdateItemGroupRename(_IDItemGroup, textBoxNewName.Text);
                        break;
                    case 2: // new
                        PublicShare.UpdateItemGroupNew(textBoxNewName.Text);
                        break;
                }
                this.Close();
            }
            catch (Exception ex)
            {
                using (WindowDisplayMessage wdm = new WindowDisplayMessage(ex.Message, 2))
                {
                    wdm.ShowDialog();
                    wdm.Dispose();
                    wdm.Owner = this;
                }
            }
        }

        private void textBoxNewName_GotFocus(object sender, RoutedEventArgs e)
        {
            textBlockNewNameTips.Visibility = Visibility.Hidden;
        }

        private void textBoxNewName_LostFocus(object sender, RoutedEventArgs e)
        {
            textBlockNewNameTips.Visibility = (textBoxNewName.Text.Length > 0) ? Visibility.Hidden : Visibility.Visible;
        }
    }
}
