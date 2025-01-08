using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using log4net;
using log4net.Config;
using System.Reflection;
using System.Threading.Tasks;


namespace IntroSE.Kanban.Backend.DataAccessLayer
{
    public abstract class DalController
    {
        //fields
        protected readonly string _connectionString;
        private readonly string _tableName;

        //logger
        private readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);


        //constructor
        public DalController(string tableName)
        {
            string startupPath = Path.Combine(Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName, "kanban.db");
            this._connectionString = $"Data Source={startupPath}; Version=3;";
            this._tableName = tableName; 
            
            //logging setup
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
        }


        //methods


        /// <summary>
        /// This method is responsible for updating the specified database entry
        /// </summary>
        /// <param name="id">id of entry to update</param>
        /// <param name="attributeName">name of attribute to update</param>
        /// <param name="attributeValue"> new value to update</param>
        /// <param name="kind">type of attribute</param>
        /// <returns>returns true if it was successful, false otherwise</returns>
        public bool Update(long id, string attributeName, string attributeValue, string kind)
        {
            int res = -1;
            using (var connection = new SQLiteConnection(_connectionString))
            {
                SQLiteCommand command = new SQLiteCommand
                {
                    Connection = connection,
                    CommandText = $"update {_tableName} set [{attributeName}]=@{attributeName} where {kind}={id}"
                };
                try
                {
                    log.Info("Attempting to open connection and update " + attributeName + " in table " + _tableName + " in database");

                    command.Parameters.Add(new SQLiteParameter(attributeName, attributeValue));
                    
                    connection.Open();
                    res = command.ExecuteNonQuery();
                    
                    log.Debug("Successfully updated entry in table " + _tableName);
                }
                catch
                {
                    //check if error
                    log.Error("Updating entry in table " + _tableName + " was unsuccessful for some reason");
                }
                finally
                {
                    log.Debug("Disposing command and closing connection in DalController");

                    command.Dispose();
                    connection.Close();
                }
            }
            return res > 0;
        }


        /// <summary>
        /// This method is responsible for updating a specified database entry
        /// </summary> 
        /// <param name="id">id of entry to update</param>
        /// <param name="attributeName">name of attribute to update</param>
        /// <param name="attributeValue"> new value to update</param>
        /// <param name="kind">type of attribute</param>
        /// <returns>returns true if it was successful, false otherwise</returns>
        public bool Update(long id, string attributeName, long attributeValue, string kind)
        {
            int res = -1;
            using (var connection = new SQLiteConnection(_connectionString))
            {
                SQLiteCommand command = new SQLiteCommand
                {
                    Connection = connection,
                    CommandText = $"update {_tableName} set [{attributeName}]=@{attributeName} where {kind}='{id}'"
                };
                try
                {
                    log.Info("Attempting to open connection and update " + attributeName + " in table " + _tableName + " in database");

                    command.Parameters.Add(new SQLiteParameter(attributeName, attributeValue));
                    
                    connection.Open();
                    res = command.ExecuteNonQuery();

                    log.Debug("Successfully updated entry in table " + _tableName);
                }
                catch
                {
                    //check if error
                    log.Error("Updating entry in table " + _tableName + " was unsuccessful for some reason");
                }
                finally
                {
                    log.Debug("Disposing command and closing connection in DalController");
                    
                    command.Dispose();
                    connection.Close();
                }
            }
            return res > 0;
        }


        /// <summary>
        /// This method is responsible for updating a specified database entry
        /// </summary> 
        /// <param name="id">id of entry to update</param>
        /// <param name="attributeName">name of attribute to update</param>
        /// <param name="attributeValue"> new value to update</param>
        /// <param name="kind">type of attribute</param>
        /// <returns>returns true if it was successful, false otherwise</returns>
        public bool Update(string id, string attributeName, string attributeValue, string kind)
        {
            int res = -1;
            using (var connection = new SQLiteConnection(_connectionString))
            {
                SQLiteCommand command = new SQLiteCommand
                {
                    Connection = connection,
                    CommandText = $"update {_tableName} set [{attributeName}]=@{attributeName} where {kind}='{id}'"
                };
                try
                {
                    log.Info("Attempting to open connection and update " + attributeName + " in table " + _tableName + " in database");
                    
                    command.Parameters.Add(new SQLiteParameter(attributeName, attributeValue));

                    connection.Open();
                    res = command.ExecuteNonQuery();
                    
                    log.Debug("Successfully updated entry in table " + _tableName);
                }
                catch
                {
                    //check if error
                    log.Error("Updating entry in table " + _tableName + " was unsuccessful for some reason");
                }
                finally
                {
                    log.Debug("Disposing command and closing connection in DalController");
                 
                    command.Dispose();
                    connection.Close();
                }
            }
            return res >= 0;
        }




        /// <summary>
        /// This method is responsible for updating a specified database entry
        /// </summary>
        /// <param name="id1"> id of the entry's taskId</param>
        /// <param name="id2"> id of the entry's boardId</param>
        /// <param name="attributeName"></param>
        /// <param name="attributeValue"></param>
        /// <returns>returns true if it was successful, false otherwise</returns>
        public bool Update(int id1, int id2, string attributeName, long attributeValue)
        {
            int res = -1;
            using (var connection = new SQLiteConnection(_connectionString))
            {
                SQLiteCommand command = new SQLiteCommand
                {
                    Connection = connection,
                    CommandText = $"update {_tableName} set [{attributeName}]=@{attributeName} where taskId='{id1}' and boardId = '{id2}'"
                };
                try
                {
                    log.Info("Attempting to open connection and update " + attributeName + " in table " + _tableName + " in database");
                   
                    command.Parameters.Add(new SQLiteParameter(attributeName, attributeValue));
                    
                    connection.Open();
                    res = command.ExecuteNonQuery();

                    log.Debug("Successfully updated entry in table " + _tableName);
                }
                catch
                {
                    //check if error
                    log.Error("Updating entry in table " + _tableName + " was unsuccessful for some reason");
                }
                finally
                {
                    log.Debug("Disposing command and closing connection in DalController");
                    
                    command.Dispose();
                    connection.Close();
                }
            }
            return res >= 0;
        }


        /// <summary>
        /// This method is responsible for updating a specified database entry
        /// </summary>
        /// <param name="id1">id of the entry's taskId</param>
        /// <param name="id2">id of the entry's boardId</param>
        /// <param name="attributeName">name of the attribute we want to update</param>
        /// <param name="attributeValue">new value of the attribute</param>
        /// <returns>returns true if it was successful, false otherwise</returns>
        public bool Update(int id1, int id2, string attributeName, string attributeValue)
        {
            int res = -1;
            using (var connection = new SQLiteConnection(_connectionString))
            {
                SQLiteCommand command = new SQLiteCommand
                {
                    Connection = connection,
                    CommandText = $"update {_tableName} set [{attributeName}]=@{attributeName} where taskId='{id1}' and boardId = '{id2}'"
                };
                try
                {
                    log.Info("Attempting to open connection and update " + attributeName + " in table " + _tableName + " in database");
                    
                    command.Parameters.Add(new SQLiteParameter(attributeName, attributeValue));
                    
                    connection.Open();
                    res = command.ExecuteNonQuery();

                    log.Debug("Successfully updated entry in table " + _tableName);
                }
                catch
                {
                    //check if error
                    log.Error("Updating entry in table " + _tableName + " was unsuccessful for some reason");
                }
                finally
                {
                    log.Debug("Disposing command and closing connection in DalController");
                    
                    command.Dispose();
                    connection.Close();
                }
            }
            return res >= 0;
        }

        
        /// <summary>
        /// This method is responsible for updating a specified database entry
        /// </summary>
        /// <param name="id1">id of the entry's taskId</param>
        /// <param name="id2">id of the entry's boardId</param>
        /// <param name="attributeName">name of the attribute we want to update</param>
        /// <param name="attributeValue">new value of the attribute</param>
        /// <returns>returns true if it was successful, false otherwise</returns>
        public bool Update(int id1, int id2, string attributeName, DateTime attributeValue)
        {
            int res = -1;
            using (var connection = new SQLiteConnection(_connectionString))
            {
                SQLiteCommand command = new SQLiteCommand
                {
                    Connection = connection,
                    CommandText = $"update {_tableName} set [{attributeName}]=@{attributeName} where taskId='{id1}' and boardId = '{id2}'"
                };
                try
                {
                    log.Info("Attempting to open connection and update " + attributeName + " in table " + _tableName + " in database");
                 
                    command.Parameters.Add(new SQLiteParameter(attributeName, attributeValue));
                    
                    connection.Open();
                    res = command.ExecuteNonQuery();

                    log.Debug("Successfully updated entry in table " + _tableName);
                }
                catch
                {
                    //check if error
                    log.Error("Updating entry in table " + _tableName + " was unsuccessful for some reason");
                }
                finally
                {
                    log.Debug("Disposing command and closing connection in DalController");
                    
                    command.Dispose();
                    connection.Close();
                }
            }
            return res >= 0;
        }




        /// <summary>
        /// This method is responsible for updating a specified database entry
        /// </summary>
        /// <param name="id1">string of the entry's email</param>
        /// <param name="id2">id of the entry's boardId</param>
        /// <param name="attributeName">name of the attribute we want to update</param>
        /// <param name="attributeValue">new value of the attribute</param>
        /// <returns>returns true if it was successful, false otherwise</returns>
        public bool Update(string id1, int id2, string attributeName, string attributeValue)
        {
            int res = -1;
            using (var connection = new SQLiteConnection(_connectionString))
            {
                SQLiteCommand command = new SQLiteCommand
                {
                    Connection = connection,
                    CommandText = $"update {_tableName} set [{attributeName}]=@{attributeName} where email='{id1}' and boardId = '{id2}'"
                };
                try
                {
                    log.Info("Attempting to open connection and update " + attributeName + " in table " + _tableName + " in database");
                 
                    command.Parameters.Add(new SQLiteParameter(attributeName, attributeValue));
                    
                    connection.Open();
                    res = command.ExecuteNonQuery();

                    log.Debug("Successfully updated entry in table " + _tableName);
                }
                catch
                {
                    //check if error
                    log.Error("Updating entry in table " + _tableName + " was unsuccessful for some reason");
                }
                finally
                {
                    log.Debug("Disposing command and closing connection in DalController");
                    
                    command.Dispose();
                    connection.Close();
                }
            }
            return res >= 0;
        }




        /// <summary>
        /// This method is responsible for updating a specified database entry
        /// </summary>
        /// <param name="attributeName">name of the attribute we want to update</param>
        /// <param name="attributeValue">new value we want to insert into the database</param>
        /// <returns>returns true if it was successful, false otherwise</returns>
        public bool Update(string attributeName, int attributeValue)
        {
            int res = -1;
            using (var connection = new SQLiteConnection(_connectionString))
            {
                SQLiteCommand command = new SQLiteCommand
                {
                    Connection = connection,
                    CommandText = $"update {_tableName} set [{attributeName}]=@{attributeName} "
                };
                try
                {
                    log.Info("Attempting to open connection and update " + attributeName + " in table " + _tableName + " in database");
        
                    command.Parameters.Add(new SQLiteParameter(attributeName, attributeValue));
                    
                    connection.Open();
                    res = command.ExecuteNonQuery();
                    log.Debug("Successfully updated entry in table " + _tableName);
                }
                catch
                {
                    //check if error
                    log.Error("Updating entry in table " + _tableName + " was unsuccessful for some reason");
                }
                finally
                {
                    log.Debug("Disposing command and closing connection in DalController");
                    
                    command.Dispose();
                    connection.Close();
                }
            }
            return res > 0;
        }


        /// <summary>
        /// This method is responsible for selecting the data from a database table
        /// </summary>
        /// <returns>returns a list of dto's</returns>
        public List<DTO> Select()
        {
            List<DTO> results = new List<DTO>();
            using (var connection = new SQLiteConnection(_connectionString))
            {
                SQLiteCommand command = new SQLiteCommand(null, connection);
                command.CommandText = $"select * from {_tableName};";
                SQLiteDataReader dataReader = null;
                try
                {
                    log.Info("Attempting to open connection with database and select data from " + _tableName);
        
                    connection.Open();
                    dataReader = command.ExecuteReader();

                    while (dataReader.Read())
                    {
                        results.Add(ConvertReaderToObject(dataReader));

                    }
                    
                    log.Debug("Successfully selected all entries from " + _tableName);
                }
                catch
                {
                    //check if error
                    log.Error("Selecting entries from table " + _tableName + " from the database was unsuccessful for some reason");
                }
                finally
                {
                    if (dataReader != null)
                    {
                        log.Debug("Closing dataReader in DalController");
                    
                        dataReader.Close();
                    }

                    log.Debug("Disposing command and closing connection in DalController");
                    
                    command.Dispose();
                    connection.Close();
                }
            }
            return results;
        }

        /// <summary>
        /// Selects entries from the database
        /// </summary>
        /// <param name="assignee">The user we want to extract their assigned tasks from the database</param>
        /// <param name="boardId">the id of the board we want to extract the tasks from</param>
        /// <returns>a list of DTOs</returns>
        public List<DTO> Select(string assignee,int boardId)
        {
            List<DTO> results = new List<DTO>();
            using (var connection = new SQLiteConnection(_connectionString))
            {
                SQLiteCommand command = new SQLiteCommand(null, connection);
                command.CommandText = $"select * from {_tableName} where boardId = {boardId} and assignee = {assignee}";
                SQLiteDataReader dataReader = null;
                try
                {
                    log.Info("Attempting to open connection with database and select data from " + _tableName);

                    connection.Open();
                    dataReader = command.ExecuteReader();

                    while (dataReader.Read())
                    {
                        results.Add(ConvertReaderToObject(dataReader));

                    }

                    log.Debug("Successfully selected all entries from " + _tableName);
                }
                catch
                {
                    //check if error
                    log.Error("Selecting entries from table " + _tableName + " from the database was unsuccessful for some reason");
                }
                finally
                {
                    if (dataReader != null)
                    {
                        log.Debug("Closing dataReader in DalController");

                        dataReader.Close();
                    }

                    log.Debug("Disposing command and closing connection in DalController");

                    command.Dispose();
                    connection.Close();
                }
            }
            return results;
        }


        /// <summary>
        /// This method gets the data from the database and turns it into an object that we can use
        /// </summary>
        /// <param name="reader">the sql reader that translates the database data</param>
        /// <returns>returns a DTO</returns>
        protected abstract DTO ConvertReaderToObject(SQLiteDataReader reader);


        /// <summary>
        /// This method is responsible for deleting data from the database
        /// </summary>
        /// <param name="boardId">an id of the board we want to delete</param>
        /// <returns>returns true if it was successful</returns>
        public bool Delete(long boardId) // deletion of board
        {
            int res = -1;
            using (var connection = new SQLiteConnection(_connectionString))
            {
                var command = new SQLiteCommand
                {
                    Connection = connection,
                    CommandText = $"delete from {_tableName} where boardId='{boardId}'"
                };
                try
                {
                    log.Info("Attempting to open connecting with database and delete an entry from " + _tableName);
        
                    connection.Open();
                    res = command.ExecuteNonQuery();
                    
                    log.Debug("Successfully deleted entry from table " + _tableName);
                }
                catch
                {
                    //check if error
                    log.Error("Deleting entry in table " + _tableName + " was unsuccessful for some reason!");
                }
                finally
                {
                    log.Debug("Disposing command and closing connection in DalController");
                    
                    command.Dispose();
                    connection.Close();
                }
            }
            return res >= 0;
        }


        /// <summary>
        /// This method is responsible for deleting data from the database
        /// </summary>
        /// <param name="email">an email of the user we want to delete</param>
        /// <returns>returns true if it was successful</returns>
        public bool Delete(string email) // deletion of emailId
        {
            int res = -1;
            using (var connection = new SQLiteConnection(_connectionString))
            {
                var command = new SQLiteCommand
                {
                    Connection = connection,
                    CommandText = $"delete from {_tableName} where email={email}"
                };
                try
                {
                    log.Info("Attempting to open connecting with database and delete an entry from " + _tableName);
        
                    connection.Open();
                    res = command.ExecuteNonQuery();
                    
                    log.Debug("Successfully deleted entry from table " + _tableName);
                }
                catch
                {
                    //check if error
                    log.Error("Deleting entry in table " + _tableName + " was unsuccessful for some reason!");
                }
                finally
                {
                    log.Debug("Disposing command and closing connection in DalController");
                    
                    command.Dispose();
                    connection.Close();
                }
            }
            return res > 0;
        }


        /// <summary>
        /// This method is responsible for deleting data from the database
        /// </summary>
        /// <param name="boardId">an id of a board that the task is part of</param>
        /// <param name="taskId">an id of a task we want to delete</param>
        /// <returns>returns true if it was successful</returns>
        public bool Delete(long boardId, long taskId) // deletion of a task
        {
            int res = -1;
            using (var connection = new SQLiteConnection(_connectionString))
            {
                var command = new SQLiteCommand
                {
                    Connection = connection,
                    CommandText = $"delete from {_tableName} where boardId={boardId} and taskId={taskId}"
                };
                try
                {
                    log.Info("Attempting to open connecting with database and delete an entry from " + _tableName);
        
                    connection.Open();
                    res = command.ExecuteNonQuery();
                    
                    log.Debug("Successfully deleted entry from table " + _tableName);
                }
                catch
                {
                    //check if error
                    log.Error("Deleting entry in table " + _tableName + " was unsuccessful for some reason!");
                }
                finally
                {
                    log.Debug("Disposing command and closing connection in DalController");
                    
                    command.Dispose();
                    connection.Close();
                }
            }
            return res > 0;
        }


        /// <summary>
        /// This method is responsible for deleting data from the database
        /// </summary>
        /// <param name="email">an email of the user we want to delete</param>
        /// <param name="boardId">an id of a board that we want to delete user from</param>
        /// <returns>returns true if it was successful</returns>
        public bool Delete(string email, long boardId) // deletion of a user from a board
        {
            int res = -1;
            using (var connection = new SQLiteConnection(_connectionString))
            {
                var command = new SQLiteCommand
                {
                    Connection = connection,
                    CommandText = $"delete from {_tableName} where boardId='{boardId}' and email='{email}'"
                };
                try
                {
                    log.Info("Attempting to open connecting with database and delete an entry from " + _tableName);
        
                    connection.Open();
                    res = command.ExecuteNonQuery();
                    
                    log.Debug("Successfully deleted entry from table " + _tableName);
                }
                catch
                {
                    //check if error
                    log.Error("Deleting entry in table " + _tableName + " was unsuccessful for some reason!");
                }
                finally
                {
                    log.Debug("Disposing command and closing connection in DalController");
                    
                    command.Dispose();
                    connection.Close();
                }
            }
            return res > 0;
        }

        /// <summary>
        /// Deletes table content
        /// </summary>
        /// <returns>returns true if the deletion was successful and false otherwise</returns>
        public bool Delete() // delete table content
        {
            int res = -1;
            using (var connection = new SQLiteConnection(_connectionString))
            {
                var command = new SQLiteCommand
                {
                    Connection = connection,
                    CommandText = $"delete from {_tableName} "
                };
                try
                {
                    log.Info("Attempting to open connecting with database and delete content from  " + _tableName + " table");

                    connection.Open();
                    res = command.ExecuteNonQuery();

                    log.Debug("Successfully deleted " + _tableName + "table");
                }
                catch
                {
                    //check if error
                    log.Error("Deleting table " + _tableName + " was unsuccessful for some reason!");
                }
                finally
                {
                    log.Debug("Disposing command and closing connection in DalController");

                    command.Dispose();
                    connection.Close();
                }
            }
            return res >= 0;
        }
    }
}