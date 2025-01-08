using Frontend.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Frontend.ViewModel
{
    public class UserBoardViewModel : NotifiableObject
    {

        // fields
        private Model.BackendController controller;
        private UserModel user;
        private UserBoardModel userBoard;
        private bool _enableForward = false;
        private MessageModel _selectedBoards;


        // constructor
        public UserBoardViewModel(UserModel user)
        {
            this.controller = user.Controller;
            this.user = user;
            Title = "Boards for " + user.Email;
            UserBoard = controller.ListUserBoards(user.Email);
        }
        


        public SolidColorBrush BackgroundColor
        {
            get
            {
                return new SolidColorBrush(user.Email.Contains("achiya") ? Colors.Purple : Colors.MediumOrchid);
            }
        }

        // getters and setters
        public string Title { get; private set; }

        public UserBoardModel UserBoard { get; private set; }

        private BoardModel _selectedBoard;
        public BoardModel SelectedBoard
        {
            get
            {
                return _selectedBoard;
            }
            set
            {
                _selectedBoard = value;
                EnableForward = value != null;
                RaisePropertyChanged("SelectedBoard");
            }
        }

        public bool EnableForward
        {
            get => _enableForward;
            private set
            {
                _enableForward = value;
                RaisePropertyChanged("EnableForward");
            }
        }


        //methods
        public bool Logout()
        {
            try
            {
                controller.Logout(user.Email);
                return true;

            }catch (Exception ex)
            {
                return false;
            }
        }


    }
}
