using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Text.Json.Serialization;
using log4net;
using log4net.Config;
using System.IO;
using System.Reflection;
using IntroSE.Kanban.Backend.DataAccessLayer;

namespace IntroSE.Kanban.Backend.BusinessLayer
{
    public class User
    {

        /// <summary>
        /// User is an object that can be created through the userManagement, user can login/logout of the system and is
        /// the object representing the client of the project
        /// </summary>
        /// <param name="email">the email of the user</param>
        /// <param name="password">the password of the user</param>
        /// <param name="loggedIn">status of the user- logged in or not</param>

        //fields
        public string email;
        private string password;
        private bool loggedIn;

        private readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);


        //constructor
        public User(string email, string password)
        {
            email = email.ToLower();
            Email = email;
            Password = password;
            LoggedIn = false;

            //logging setup
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
            log.Info("New user is created!");
        }

        public User(string email, string password, bool isLoggedIn)
        {
            email = email.ToLower();
            Email = email;
            Password = password;
            LoggedIn = isLoggedIn;

            //logging setup
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
            log.Info("User is loaded!");
        }

        [JsonConstructor]
        public User(string email)
        {
            Email = email;
        }


        //getters and setters
        public string Email { get { return email; } set { email = value; } }

        public string Password { get { return password; } set { password = value; } }

        public bool LoggedIn { get { return loggedIn; } set { loggedIn = value; } }


        //methods

        ///<summary>
        ///This function sets the "log in" state of the user to "logged in", according to functional requirerment 8. 
        ///</summary>
        public void logIn()
        {
            log.Debug("Successfully called method logIn in User, logging in user");

            LoggedIn = true;
        }


        ///<summary>
        ///This function sets the "log in" state of the user to "not logged in", according to functional requirerment 8. 
        ///</summary>
        public void logOut()
        {
            log.Debug("Successfully called method logOut in User, logging out user.");

            LoggedIn = false;
        }


        ///<summary>
        ///This function checks if the given password is identify to user password 
        ///</summary>
        ///<param name="pass"></param>
        ///<returns>if the password matches</returns>
        public bool passwordMatch(string pass)
        {
            log.Debug("Successfully called method passwordMatch in User, checking if password matches user's password.");

            return password.Equals(pass);
        }
    }
}