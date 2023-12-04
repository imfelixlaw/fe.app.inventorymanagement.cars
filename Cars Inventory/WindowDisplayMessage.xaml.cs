using System;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Cars_Inventory
{
    /// <summary>
    /// Interaction logic for WindowDisplayMessage.xaml
    /// </summary>
    public partial class WindowDisplayMessage : Window
    {
        public WindowDisplayMessage(string Message, int Type = 0, string Title = "Message")
        {
            InitializeComponent();
            try
            {
                switch (Type) // detecting which method is called
                {
                    case 1: // Question
                        this.Title = "Question"; // windows title
                        imageApplication.Source = new BitmapImage(new Uri("/Cars%20Inventory;component/Artwork/dialog_question.ico", UriKind.Relative)); // app display icons
                        break;
                    case 2: // Warning
                        this.Title = "Warning";
                        imageApplication.Source = new BitmapImage(new Uri("/Cars%20Inventory;component/Artwork/dialog_warning.ico", UriKind.Relative));
                        break;
                    case 3: // Error
                        this.Title = "Error";
                        imageApplication.Source = new BitmapImage(new Uri("/Cars%20Inventory;component/Artwork/dialog_error.ico", UriKind.Relative));
                        break;
                    default: // Standard
                        this.Title = Title;
                        break;
                }
                textBlockMessage.Text = Message; // output the message
            }
            catch { } // just dismiss the error
        }

        private void buttonOK_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            this.Dispose(); // trying to dispose item, generally no required but double protection
        }
    }
}
