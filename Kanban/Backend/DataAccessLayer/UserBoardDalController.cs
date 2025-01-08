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
    public class UserBoardDalController : DalController
    {
        //fields
        private const string userBoardTableName = "UserBoard";

        //logger
        private readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);


        //constructor
        public UserBoardDalController() : base(userBoardTableName)
        {
            //logging setup
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
        }


        //methods


        /// <summary>
        /// This method inserts a user and board link into the userBoard table in the database
        /// </summary>
        /// <param name="ub">a link between the users and the boards for the database</param>
        /// <returns>returns true if the insertion was successful</returns>
        public bool Insert(UserBoardDTO ub)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                int res = -1;
                SQLiteCommand command = new SQLiteCommand(null, connection);
                try
                {
                    log.Info("Attempting to open connection with database and inserting a new userBoard entry in UserBoardDalController.");
                    
                    connection.Open();
                    command.CommandText = $"INSERT INTO {userBoardTableName} ({UserBoardDTO.emailColumnName}, {UserBoardDTO.boardIdColumnName},{UserBoardDTO.isOwnerColumnName}) " +
                        $"VALUES (@emailVal,@boardIdVal,@isOwnerVal);";

                    SQLiteParameter emailParam = new SQLiteParameter(@"emailVal", ub.Email);
                    
                    SQLiteParameter boardIdParam = new SQLiteParameter(@"boardIdVal", ub.BoardId);
                    
                    SQLiteParameter isOwnerParam = new SQLiteParameter(@"isOwnerVal", ub.IsOwner);


                    command.Parameters.Add(emailParam);
                    
                    command.Parameters.Add(boardIdParam);
                    
                    command.Parameters.Add(isOwnerParam);
                    
                    
                    command.Prepare();
                    res = command.ExecuteNonQuery();
                    
                    log.Debug("successfully added new userBoard entry into database.");
                }
                catch
                {
                    //could have to throw error
                    log.Error("Attempting to add new userBoard entry to database was unsuccessful for some reason");
                }
                finally
                {
                    log.Debug("Disposing command and closing connection with database in UserBoardController.");
                    
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
            log.Info("Converting database entries extracted by reader into a DTO in UserBoardDalController.");
        
            UserBoardDTO userBoardDTO = new(reader.GetString(0), reader.GetInt32(1), reader.GetString(2));
            return userBoardDTO;
        }
    }
}