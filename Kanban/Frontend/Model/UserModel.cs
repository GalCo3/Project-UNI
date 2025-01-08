namespace Frontend.Model
{
    public class UserModel : NotifiableModelObject
    {
        //fields
        private string _email;
       

        //constructor

        /// <summary>
        /// The following is a constructor of the userModel
        /// </summary>
        /// <param name="controller">controller of the backend, responsible for communcating with the backend</param>
        /// <param name="email">email of the user</param>
        public UserModel(BackendController controller, string email) : base(controller)
        {
            this.Email = email;
        }


        //getters and setters
        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                RaisePropertyChanged("Email");
            }
        }
    }
}
