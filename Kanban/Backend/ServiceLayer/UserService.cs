using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntroSE.Kanban.Backend.BusinessLayer;
using log4net;
using log4net.Config;
using System.IO;
using System.Reflection;


namespace IntroSE.Kanban.Backend.ServiceLayer
{
    public class UserService
    {
   

        //fields
        private UserManagement um;
        private readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);


        //constructor
        public UserService(UserManagement um)
        {
            this.um = um;

            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));

        }

        //constructor
        public UserService() {
            um = new UserManagement();

            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
        }

        public UserManagement UserManagement { get { return um; } }


        /// <summary>
        ///This function is responsible for functional requirement 7, allowing the program to register new users.
        /// </summary>
        /// <param name="email">an email of the user</param>
        /// <param name="password">the password of the user</param>
        /// <returns>a json string for creating a new user in the system</returns>
        public string createNewUser(string email, string password)
        {
            try
            {
                log.Info("Attempting to create a new user");

                um.createNewUser(email, password);

                log.Debug("Created new user successfully");

                Response<string> r = new Response<string>(null, null);
                return JsonController.Serialize(r);

            }
            catch (Exception e)
            {
                log.Error("User not created due to error: " + e.Message);

                Response<string> r = new Response<string>(e.Message, null);
                string json = JsonController.Serialize(r);
                return json;
            }
        }


        /// <summary>
        ///This function is responsible for functional requirement 8, allowing registered users to login.
        /// </summary>
        /// <param name="email">the email of the user</param>
        /// <param name="password">the password of the user</param>
        /// <returns>a json string for logging in a user in the system</returns>
        public string login(string email, string password)
        {
            try
            {
                log.Info("Attempting to log in user");

                um.logIn(email, password);

                log.Debug("User was logged in successfully");

                Response<string> r = new Response<string>(null, email);
                string json = JsonController.Serialize(r);
                return json;
            }
            catch (Exception e)
            {
                log.Error("User failed to log in due to error: " + e.Message);

                Response<string> r = new Response<string>(e.Message, null);
                string json = JsonController.Serialize(r);
                return json;
            }
        }


        /// <summary>
        ///This function is responsible for functional requirement 8, allowing logged in users to logout.
        /// </summary>
        /// <param name="email">the email of the user</param>
        /// <returns>a json string for logging out a user in the system</returns>
        public string logout(string email)
        {
            try
            {
                log.Info("Attempting to log out user");

                um.logOut(email);

                log.Debug("User logged out successfully");

                Response<string> r = new Response<string>(null, null);
                return JsonController.Serialize(r);
            }
            catch (Exception e)
            {
                log.Error("User failed to log out due to error: " + e.Message);

                Response<string> r = new Response<string>(e.Message, null);
                string json = JsonController.Serialize(r);
                return json;
            }
        }


        /// <summary>
        ///This method is an assisting method for functional requirement 8, validating that a user is logged in for them to logout, as well as other methods that require a user to be logged in 
        /// </summary>
        /// <param name="email">the email of the user</param>
        /// <returns>a json string for checking if the user is logged in or not</returns>
        public string isLoggedIn(string email)
        {
            try
            {
                log.Info("Attempting to verify if user is logged in");

                um.isLoggedIn(email);

                log.Debug("Verification of user being logged in was successfull");

                Response<string> r = new Response<string>(null, "User with email " + email + "is logged in");
                string json = JsonController.Serialize(r);
                return json;
            }
            catch (Exception e)
            {
                log.Error("Verification of user being logged in failed due to error: " + e.Message);

                Response<string> r = new Response<string>(e.Message, null);
                string json = JsonController.Serialize(r);
                return json;
            }
        }
    }
}
