using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using log4net.Config;
using System.Reflection;
using System.IO;

namespace IntroSE.Kanban.Backend.DataAccessLayer
{
    
    public class UserDalController:DalController
    {
        //fields
        private const string UserBoardName = "User";

        //logger
        private readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);


        //constructor
        public UserDalController() : base(UserBoardName)
        {
            //logging setup
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
        }


        //methods


        /// <summary>
        /// This method inserts a User into the user table in the database
        /// </summary>
        /// <param name="user">a userDto object for the database</param>
        /// <returns>returns true if the insertion was successful</returns>
        public bool Insert(UserDTO user)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                int res = -1;
                SQLiteCommand command = new SQLiteCommand(null, connection);
                try
                {
                    log.Info("Attempting to open connection with database and inserting a new User entry in UserDalController.");
        
                    connection.Open();
                    command.CommandText = $"INSERT INTO {UserBoardName} ({UserDTO.emailColumnName}, {UserDTO.passwordColumnName}) " +
                        $"VALUES (@emailVal,@passwordVal);";

                    SQLiteParameter emailParam = new SQLiteParameter(@"emailVal", user.Email);
                    
                    SQLiteParameter passwordParam = new SQLiteParameter(@"passwordVal", user.Password);
                    

                    command.Parameters.Add(emailParam);
                    
                    command.Parameters.Add(passwordParam);
                     
                    
                    command.Prepare();
                    res = command.ExecuteNonQuery();
                    
                    log.Debug("successfully added new User entry into database.");
                }
                catch
                {
                    //could have to throw error
                    log.Error("Attempting to add new User entry to database was unsuccessful for some reason");
                }
                finally
                {
                    log.Debug("Disposing command and closing connection with database in TaskCounterDalController.");
                    
                    command.Dispose();
                    connection.Close();
                }
                return res > 0;
            }
        }


        /// <summary>
        /// This method extracts data from the database and turns it into a DTO, (specifically used to load data into project)
        /// </summary>
        /// <param name="reader">the sql reader that translates the database data</param>
        /// <returns>returns a dto</returns>
        protected override DTO ConvertReaderToObject(SQLiteDataReader reader)
        {
            log.Info("Converting database entries extracted by reader into a DTO in UserDalController.");

            UserDTO userDTO = new(reader.GetString(0), reader.GetString(1));
            return userDTO;
        }
    }
}