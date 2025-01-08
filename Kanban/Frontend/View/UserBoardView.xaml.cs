using Frontend.Model;
using Frontend.ViewModel;
using System.Windows;

namespace Frontend.View
{
    /// <summary>
    /// Interaction logic for BoardView.xaml
    /// </summary>
    public partial class UserBoardView : Window
    {
        //fields
        private UserBoardViewModel viewModel;
        private UserModel user;


        //constructor

        /// <summary>
        /// The following is the constructor of the UserBoardView, displaying the boards of the user
        /// </summary>
        /// <param name="u"> the UserModel that is currently logged in</param>
        public UserBoardView(UserModel u)
        {
            InitializeComponent();
            this.viewModel = new UserBoardViewModel(u);
            this.DataContext = viewModel;
            user = u;
        }


        //methods

        /// <summary>
        /// This method is responsible to perfom actions after the user clicks the logout button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">making a route for the event</param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            bool isLoggedOut = viewModel.Logout();
            if (isLoggedOut)
            {
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
                this.Close();
            }
        }


        /// <summary>
        /// This method is responsible to perfom actions after the user clicks on a specific board of his to display.
        /// </summary>
        /// <param name="sender">the sender</param>
        /// <param name="e">making a route for the event</param>
        private void Board_Select(object sender, RoutedEventArgs e) {
            BoardView boardView = new BoardView(user,viewModel.SelectedBoard);
            if(viewModel.SelectedBoard != null)
            {
                boardView.Show();
                this.Close();

            }
            
        }
    }
}
