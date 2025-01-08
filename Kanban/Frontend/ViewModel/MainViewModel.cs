using Frontend.Model;
using System;

namespace Frontend.ViewModel
{
    class MainViewModel : NotifiableObject
    {
        //fields and getters and setters
        public BackendController Controller { get; private set; }

        private string _email;
        private string _password;
        private string _message;


        //constructor
        public MainViewModel()
        {
            this.Controller = new BackendController();

            this.Email = "mail@mail.com";
            this.Password = "Password1";
        }


        //getters and setters

        public string Email
        {
            get => _email;
            set
            {
                this._email = value;
                RaisePropertyChanged("Email");
            }
        }
        public string Password
        {
            get => _password;
            set
            {
                this._password = value;
                RaisePropertyChanged("Password");
            }
        }
        public string Message
        {
            get => _message;
            set
            {
                this._message = value;
                RaisePropertyChanged("Message");
            }
        }


        //methods

        /// <summary>
        /// A method that is responsible for the login operation, after the user has clicked the login button.
        /// </summary>
        /// <returns></returns>
        public UserModel Login()
        {
            Message = "";
            try
            {
                return Controller.Login(Email, Password);
            }
            catch (Exception e)
            {
                Message = e.Message;
                return null;
            }
        }


        /// <summary>
        /// A method that is responsible for the registration operation, after the user has clicked on the register button.
        /// </summary>
        public void Register()
        {
            Message = "";
            try
            {
                Controller.Register(Email, Password);
                Message = "Registered successfully";
            }
            catch (Exception e)
            {
                Message = e.Message;
            }
        }

    }
}
