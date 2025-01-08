using Frontend.Model;
using Frontend.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Frontend.View
{
    /// <summary>
    /// Interaction logic for BoardView.xaml
    /// </summary>
    public partial class BoardView : Window
    {
        //fields
        private BoardViewModel viewModel;
        private UserModel user;

        
        //constructor

        /// <summary>
        /// The following is a constructor that is responsible for the boardView, displaying the current board of the suer
        /// </summary>
        /// <param name="user">email of the user</param>
        /// <param name="u">the board model</param>
        public BoardView(UserModel user,BoardModel u)
        {
            InitializeComponent();
            viewModel = new BoardViewModel(u);
            this.DataContext = viewModel;
            this.user = user;
        }


        //methods

        /// <summary>
        /// This method is responsible to perfom actions after a user clicks a button.
        /// </summary>
        /// <param name="sender">the sender</param>
        /// <param name="e">making a route for the event</param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }



        /// <summary>
        /// This method is responsible to perfom actions after a user clicks a button.
        /// </summary>
        /// <param name="sender">the sender</param>
        /// <param name="e">making a route for the event</param>
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            UserBoardView userBoardView = new UserBoardView(user);
            userBoardView.Show();
            this.Close();
        }
    }
}
