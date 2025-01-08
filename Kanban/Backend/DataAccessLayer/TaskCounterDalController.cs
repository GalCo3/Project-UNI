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
    public class TaskCounterDalController:DalController
    {
        //fields
        private const string BoardTableName = "TaskIdCounter";

        //logger
        private readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        
        //constructor
        public TaskCounterDalController() : base(BoardTableName)
        {
            //logging setup
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));

        }


        //methods


        /// <summary>
        /// This method inserts a taskIdCounter entry into the taskId table in the database.
        /// </summary>
        /// <param name="taskIdCounterDTO">a task id for the database</param>
        /// <returns>returns true if the insertion was successful</returns>
        public bool Insert(TaskIdCounterDTO taskIdCounterDTO)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                int res = -1;
                SQLiteCommand command = new SQLiteCommand(null, connection);
                try
                {
                    log.Info("Attempting to open connection with database and inserting a new taskId entry in TaskCounterDalController.");
                
                    connection.Open();
                    
                    command.CommandText = $"INSERT INTO {BoardTableName} ({TaskIdCounterDTO.BoardIDColumnName},{TaskIdCounterDTO.counterColumnName}) " +
                        $"VALUES (@boardIdVal,@counterVal);";

                    SQLiteParameter boardIdParam = new SQLiteParameter(@"boardIdVal", taskIdCounterDTO.BoardId);
                    
                    SQLiteParameter counterParam = new SQLiteParameter(@"counterVal", taskIdCounterDTO.Counter);
                    
                    command.Parameters.Add(boardIdParam);
                    
                    command.Parameters.Add(counterParam);
                    

                    command.Prepare();
                    res = command.ExecuteNonQuery();
                
                    log.Debug("successfully added new taskId entry into database.");
                }
                catch
                {
                    //could have to throw error
                    log.Error("Attempting to add new taskId entry to database was unsuccessful for some reason");
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
            log.Info("Converting database entries extracted by reader into a DTO in TaskCounterDalController.");
        
            TaskIdCounterDTO taskIdCounterDTO= new TaskIdCounterDTO(reader.GetInt32(0),reader.GetInt32(1));
            return taskIdCounterDTO;
        }
    }
}