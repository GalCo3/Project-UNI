using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
//using System.Collections.Generic;
using System.Collections;
using log4net;
using log4net.Config;
using System.IO;
using System.Reflection;
using System.ComponentModel.DataAnnotations;
using IntroSE.Kanban.Backend.DataAccessLayer;

namespace IntroSE.Kanban.Backend.BusinessLayer
{

    public class UserManagement
    {
        /// <summary>
        /// Is an object representing the control and management of the users of the project.
        /// </summary>
        /// <param name="_users">a list of users</param>
        ///<param name="userDalController">a controller for a user table</param>
        ///<param name="emailCheck">an attribute to check an email</param>


        //fields
        private List<User> _users;
        private UserDalController userDalController;

        private readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        //magic numbers
        private readonly int MAX_PASSWORD_LENGTH = 20;
        private readonly int MIN_PASSWORD_LENGTH = 6;

        private EmailAddressAttribute emailCheck;


        //constructor
        public UserManagement()
        {
            _users = new List<User>();
            emailCheck = new EmailAddressAttribute();
            userDalController = new UserDalController();

            //setting logging
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
        }


        //getter and setter
        public List<User> Users { get { return _users; } set { _users = value; } }

        public UserDalController UserDalController { get { return userDalController; } set { userDalController = value; } }

        public EmailAddressAttribute EmailCheck { get { return emailCheck; } set { emailCheck = value; } }


        //methods

        /// <summary>
        /// getter method
        /// </summary>
        /// <param name="email">the email of the user</param>
        /// <returns>returns a user</returns>
        /// <exception cref="MissingFieldException">when user not found</exception>
        public User GetUser(string email)
        {
            log.Debug("successfully called method GetUser in UserManagement, searching for the user with the email: " + email);

            checks(email);
            email = email.ToLower();

            foreach (User user in _users)
            {
                if (email.Equals(user.email))
                    return user;
            }

            throw new MissingFieldException("User not found");
        }




        /// <summary>
        ///This function creates a new user in the system, according to functional requirerment 7. 
        ///We make sure that the email and password are valid 
        /// </summary>
        /// <param name="email">the email of the user</param>
        /// <param name="password">the password of the user</param>
        /// <returns>returns true if a user was created successfully</returns>
        /// <exception cref="ArgumentException">not valid input</exception>
        public bool createNewUser(string email, string password)
        {
            log.Debug("successfully called method createNewUser in UserManagement, creating a user with the email: " + email);

            if (!isValidEmail(email))
                throw new ArgumentException("email is not valid");

            email = email.ToLower();

            if (!passwordCheck(password))
                throw new ArgumentException("Password is not valid");

            if (exist(email))
                throw new ArgumentException("User already exist");

            if (userDalController.Insert(new UserDTO(email, password))) 
            { 
                _users.Add(new User(email, password));
            }
            else
            {
                throw new Exception("Failed to add user to the database");
            }
            

            return true;

        }


        /// <summary>
        ///This function logs the user in by the given email and password, according to functional requirerment 8, and makes sure it is logged out first. It validates the password tooand checks that the email is valid as well.
        /// </summary>
        /// <param name="email">the email of the user</param>
        /// <param name="password">the password of the user</param>
        /// <returns>true if user logged in</returns>
        /// <exception cref="ArgumentException">password isnt valid</exception>
        /// <exception cref="Exception">user already logged in</exception>
        public bool logIn(string email, string password)
        {
            log.Debug("successfully called method logIn in UserManagement, that logs in the user with the email: " + email);

            checks(email);
            email = email.ToLower();

            if (!passwordCheck(password))
                throw new ArgumentException("password not valid");

            User toLogIn = GetUser(email);

            if (toLogIn.LoggedIn)
                throw new Exception("User already logged in");

            if (toLogIn.passwordMatch(password))
            {
                toLogIn.logIn();
                return true;

            }

            throw new Exception("Password not correct ");
        }


        /// <summary>
        ///This function logs out of the user by the given email, according to functional requirerment 8, and makes sure it is logged in first. It checks that the email is valid.
        /// </summary>
        /// <param name="email">the email of the user</param>
        /// <returns>true if user logged out</returns>
        /// <exception cref="Exception">user isnt logged in</exception>
        public bool logOut(string email)
        {
            log.Debug("successfully called method logOut in UserManagement, that logs out the user with the email: " + email);

            checks(email);
            email = email.ToLower();

            User toLoogOut = GetUser(email);

            if (!toLoogOut.LoggedIn)
                throw new Exception("User is not logged in");

            toLoogOut.logOut();

            return true;
        }


        /// <summary>
        ///This function checks if a user with the given email is exists or not.
        ///</summary>
        /// <param name="email">the email of the user</param>
        /// <returns>true if user exists</returns>
        /// <exception cref="MissingFieldException">email isnt valid</exception>
        public bool exist(string email)
        {
            log.Debug("successfully called method exist in UserManagement, that checks if a user with the email: " + email + " exists");

            if (!isValidEmail(email))
                throw new MissingFieldException("email is not valid");

            email = email.ToLower();

            if (_users.Count == 0)
            {
            return false;
            }

            foreach (User user in _users)
            {

                if (user.Email.Equals(email))
                    return true;
            }
            return false;
        }


        /// <summary>
        ///This function checks that the email is valid and that the user exists.
        ///</summary>
        /// <param name="email">the email of the user</param>
        /// <exception cref="ArgumentException">email isnt valid</exception>
        /// <exception cref="MissingMemberException">user doesnt exists</exception>
        public void checks(string email)
        {
            log.Debug("successfully called method checks in UserManagement, that checks if an email is valid");

            if (email == null | !isValidEmail(email))
                throw new ArgumentException("email is not valid");

            email = email.ToLower();

            if (!exist(email))
                throw new MissingMemberException("User does not exist");
        }


        /// <summary>
        ///This function checks that the email is valid, by using regex.
        ///</summary>
        /// <param name="email">the email of the user</param>
        /// <returns>true if the email is valid</returns>
        public bool isValidEmail(string email)
        {
            log.Debug("Successfully called method isValidEmail in UserManagement, validating user email.");

            if (email==null)
            {
                throw new Exception("Email is null");
            }

            email = email.ToLower();
            Regex validateEmailRegex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            Regex validateEmailRegex2 = new Regex(@"^\w+([.-]?\w+)@\w+([.-]?\w+)(.\w{2,3})+$");
            return validateEmailRegex.IsMatch(email)&validateEmailRegex2.IsMatch(email)&emailCheck.IsValid(email);
        }


        /// <summary>
        ///This function checks the validity of the given password in order to allow a succesfull registration. 
        /// </summary>
        /// <param name="password">the password of the user</param>
        /// <returns>true if the password is valid</returns>
        /// <exception cref="Exception">password isnt valid</exception>
        public bool passwordCheck(string password)
        {
            log.Debug("successfully called method passwordCheck in UserManagement, that checks if a password is valid");

            if (password == null)
                throw new ArgumentNullException("password is null");

            if (password.Length < MIN_PASSWORD_LENGTH | password.Length > MAX_PASSWORD_LENGTH)
                throw new ArgumentException("Password illegal");

            int upper = 0;
            int lower = 0;
            int num = 0;

            for (int i = 0; i < password.Length; i++)
            {

                char ch = password[i];
                if (char.IsUpper(ch))
                    upper = upper + 1;
                if (char.IsLower(ch))
                    lower = lower + 1;
                if (char.IsNumber(ch))
                    num = num + 1;

            }

            if (num > 0 & upper > 0 & lower > 0)
            {
                return true;

            }

            throw new ArgumentException("Password illegal");
        }




        /// <summary>
        ///This function checks if the user is logged in or not.
        /// </summary>
        /// <param name="email">the email of the user</param>
        /// <returns>true if user logged in</returns>
        /// <exception cref="Exception">user isnt logged in</exception>
        public bool isLoggedIn(string email)
        {
            log.Debug("successfully called method isLoggedIn in UserManagement, that checks if the user with the email: " + email + " is logged in");

            checks(email);
            email = email.ToLower();

            foreach (User user in _users)
            {
                if (user.LoggedIn & user.Email.Equals(email))
                    return true;
            }

            throw new Exception("User is not Logged in");
        }




        /// <summary>
        /// This function loads data from the database into our project
        /// </summary>
        /// <returns>a string value</returns>
        public string LoadData() 
        {
            log.Debug("Successfully called method LoadData in UserManagement, loading data from database into UserManagement variables");

            List<DTO> dTOs = userDalController.Select();

            foreach (DTO dto in dTOs) 
            { 
                UserDTO dtoe = (UserDTO)dto;

                //checks if the user already exists or not
                if (!exist(dtoe.Email))
                    {
                        _users.Add(new User(dtoe.Email, dtoe.Password,false));
                    }
            }
            
            return "";
        }





        /// <summary>
        /// This function removes data from our project
        /// </summary>
        /// <returns>a string value</returns>
        public string RemoveData()
        {
            log.Debug("Successfully called method RemoveData in UserManagement, clearing all data in UserManagement");

            if (userDalController.Delete())
            {
                _users.Clear();
                return "";
            }
            throw new Exception("Database faild to remove User table");
        }
    }
}
