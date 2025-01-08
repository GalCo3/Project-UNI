using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using log4net;
using log4net.Config;


namespace IntroSE.Kanban.Backend.DataAccessLayer
{
    public class BoardDalController : DalController
    {
        //fields
        private const string BoardTableName = "Board";

        private readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);


        //constructor
        public BoardDalController() : base(BoardTableName) {

            //logging setup
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
        }


        //methods


        /// <summary>
        /// This method inserts a board entry into the board table in the database
        /// </summary>
        /// <param name="board">a board DTO for the database</param>
        /// <returns>returns true if the insertion was successful</returns>
        public bool Insert(BoardDTO board)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                int res = -1;
                SQLiteCommand command = new SQLiteCommand(null, connection);
                try
                {
                    log.Info("Attempting to open connection with database and inserting a new board entry in BoardDalController.");

                    connection.Open();
                    command.CommandText = $"INSERT INTO {BoardTableName} ({BoardDTO.boardNameColumnName}, {BoardDTO.ownerNameColumnName},{BoardDTO.idColumnName} , {BoardDTO.backLogCapacity_columnName} , {BoardDTO.inProgressCapacity_columnName} , {BoardDTO.doneCapacity_columnName}) " +
                        $"VALUES (@boardNameVal,@ownerVal,@boardIdVal,@backLogCapacity,@inProgressCapacity,@doneCapacity);";

                    SQLiteParameter boardNameParam = new SQLiteParameter(@"boardNameVal", board.BoardName);

                    SQLiteParameter ownerParam = new SQLiteParameter(@"ownerVal", board.Owner);

                    SQLiteParameter boardIdParam = new SQLiteParameter(@"boardIdVal", board.BoardId);

                    SQLiteParameter backLogCapacityParam = new SQLiteParameter(@"backLogCapacity", board.BackLogCapacity);

                    SQLiteParameter inProgressCapacityParam = new SQLiteParameter(@"inProgressCapacity", board.InProgressCapacity);

                    SQLiteParameter doneCapacityParam = new SQLiteParameter(@"doneCapacity", board.DoneCapacity);


                    command.Parameters.Add(boardNameParam);

                    command.Parameters.Add(ownerParam);

                    command.Parameters.Add(boardIdParam);

                    command.Parameters.Add(backLogCapacityParam);

                    command.Parameters.Add(inProgressCapacityParam);

                    command.Parameters.Add(doneCapacityParam);


                    command.Prepare();
                    res = command.ExecuteNonQuery();

                    log.Debug("successfully added new board entry into database.");
                }
                catch
                {
                    //could have to throw error
                    log.Error("Attempting to add new board entry to database was unsuccessful for some reason");
                }
                finally
                {
                    log.Debug("Disposing command and closing connection with database in BoardDalController.");

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
            log.Info("Converting database entries extracted by reader into a DTO in BoardDalController.");

            BoardDTO boardDTO = new(reader.GetString(0), reader.GetString(1),reader.GetInt32(2),reader.GetInt32(3),reader.GetInt32(4),reader.GetInt32(5));
            return boardDTO;
        } 
    }
}
