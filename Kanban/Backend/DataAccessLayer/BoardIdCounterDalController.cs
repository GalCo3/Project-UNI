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
    public class BoardIdCounterDalController:DalController
    {
        //fields
        private const string BoardTableName = "BoardIdCounter";

        //logger
        private readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);


        //constructor
        public BoardIdCounterDalController() : base(BoardTableName)
        {
            //logging setup
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
        }


        //methods


        /// <summary>
        /// This method inserts a boardId entry into the boardId table in the database
        /// </summary>
        /// <param name="counter">an id for the database</param>
        /// <returns>returns true if the insertion was successful</returns>
        public bool Insert(BoardIdCounterDTO counter)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                int res = -1;
                SQLiteCommand command = new SQLiteCommand(null, connection);
                try
                {
                    log.Info("Attempting to open connection with database and inserting a new boardId entry in BoardIdCounterDalController.");

                    connection.Open();
                    command.CommandText = $"INSERT INTO {BoardTableName} ({BoardIdCounterDTO.counterColumnName}) " +
                        $"VALUES (@counterVal);";

                    SQLiteParameter counterParam = new SQLiteParameter(@"counterVal", counter.Counter);

                    command.Parameters.Add(counterParam);


                    command.Prepare();
                    res = command.ExecuteNonQuery();

                    log.Debug("successfully added new boardId entry into database.");
                }
                catch
                {
                    //could have to throw error
                    log.Error("Attempting to add new boardId entry to database was unsuccessful for some reason");
                }
                finally
                {
                    log.Debug("Disposing command and closing connection with database in BoardIdCounterDalController.");

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
            log.Info("Converting database entries extracted by reader into a DTO in BoardIdCounterDalController.");

            BoardIdCounterDTO boardIdCounterDTO = new BoardIdCounterDTO(reader.GetInt32(0));
            return boardIdCounterDTO;
        }
    }
}
