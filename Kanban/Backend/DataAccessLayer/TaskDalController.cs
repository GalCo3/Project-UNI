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
    public class TaskDalController : DalController
    {
        //fields
        private const string taskBoardName = "Task";
     
        //logger
        private readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);


        //constructor
        public TaskDalController() : base(taskBoardName)
        {
            //logging setup
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
        }


        //methods

        /// <summary>
        /// This method inserts a Task into the task table in the database
        /// </summary>
        /// <param name="task">a task for the database</param>
        /// <returns>returns true if the insertion was successful</returns>
        public bool Insert(TaskDTO task)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                int res = -1;

                SQLiteCommand command = new SQLiteCommand(null, connection);
                
                try
                {
                    log.Info("Attempting to open connection with database and inserting a new task entry in TaskDalController.");
                 
                    connection.Open();
                    command.CommandText = $"INSERT INTO {taskBoardName} ({TaskDTO.taskIdColumnName}, {TaskDTO.titleColumnName},{TaskDTO.descriptionColumnName},{TaskDTO.creationTimeColumnName},{TaskDTO.dueDateColumnName},{TaskDTO.boardIdColumnName},{TaskDTO.columnIndexColumnName},{TaskDTO.assigneeColumnName}) " +
                        $"VALUES (@taskIdVal,@titleVal,@decsVal,@creationTimeVal,@dueDateVal,@boardIdVal,@columnIndexVal,@assigneeVal);";

                    SQLiteParameter taskIdParam = new SQLiteParameter(@"taskIdVal", task.TaskId);
                
                    SQLiteParameter titleParam = new SQLiteParameter(@"titleVal", task.Title);
                    
                    SQLiteParameter decsParam = new SQLiteParameter(@"decsVal", task.Description);
                    
                    SQLiteParameter creationTimeParam = new SQLiteParameter(@"creationTimeVal", task.CreationTime);
                    
                    SQLiteParameter dueDateParam = new SQLiteParameter(@"dueDateVal", task.DueDate);
                    
                    SQLiteParameter boardIdParam = new SQLiteParameter(@"boardIdVal", task.BoardId);
                    
                    SQLiteParameter columnIndexParam = new SQLiteParameter(@"columnIndexVal", task.ColumnIndex);
                    
                    SQLiteParameter assigneeParam = new SQLiteParameter(@"assigneeVal", task.Assignee);


                    command.Parameters.Add(taskIdParam);

                    command.Parameters.Add(titleParam);

                    command.Parameters.Add(decsParam);

                    command.Parameters.Add(creationTimeParam);

                    command.Parameters.Add(dueDateParam);

                    command.Parameters.Add(boardIdParam);

                    command.Parameters.Add(columnIndexParam);

                    command.Parameters.Add(assigneeParam);


                    command.Prepare();
                    res = command.ExecuteNonQuery();
                    
                    log.Debug("successfully added new task entry into database.");
                }
                catch
                {
                    //could have to throw error
                    log.Error("Attempting to add new task entry to database was unsuccessful for some reason");
                }
                finally
                {
                    log.Debug("Disposing command and closing connection with database in TaskDalController.");
                    
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
            log.Info("Converting database entries extracted by reader into a DTO in TaskDalController.");
        
            string assignee = "";

            if (reader.IsDBNull(7))
            {
                assignee = null;
            }
            else
            {
                assignee = reader.GetString(7);
            }
            
            TaskDTO taskDTO = new TaskDTO(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetDateTime(3), reader.GetDateTime(4), reader.GetInt32(5), reader.GetInt32(6), assignee);
            return taskDTO;
        }
    }
}