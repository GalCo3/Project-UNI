using Frontend.Model;
using Frontend.ViewModel;
using System.Windows;

namespace Frontend.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //fields
        private MainViewModel viewModel;


        //constructor
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new MainViewModel();
            this.viewModel = (MainViewModel)DataContext;

        }


        //methods

        /// <summary>
        /// This method is responsible to perfom actions after a user clicks a button.
        /// </summary>
        /// <param name="sender">the sender</param>
        /// <param name="e">making a route for the event</param>
        private void Login_Click(object sender, RoutedEventArgs e)
        {
            UserModel u = viewModel.Login();
            if (u != null)
            {
                UserBoardView boardView = new UserBoardView(u);
                boardView.Show();
                this.Close();
            }
        }



        /// <summary>
        /// This method is responsible to perfom actions after a user clicks a button.
        /// </summary>
        /// <param name="sender">the sender</param>
        /// <param name="e">making a route for the event</param>
        private void Register_Click(object sender, RoutedEventArgs e)
        {
            viewModel.Register();
        }

        private void TextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {

        }
    }
}
