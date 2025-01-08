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
#nullable enable
{
    public class BoardService
    {
        /// <summary>
        /// The following ServiceClass is responsible for the Boards and their functionality as requested by the user
        /// </summary>


        //fields
        private BoardManagement bm;

        private readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);


        //constructor
        public BoardService(UserManagement um)
        {
            this.bm = new BoardManagement(um);

            //logging setup
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
        }


        // constructor
        public BoardService(UserService userService)
        {
            bm = new BoardManagement(userService.UserManagement);
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
        }



        //methods

        /// <summary>
        ///This method is responsible for functional requirerment 9 and 10, allowing a user to create a board and be titled as its owner.
        /// By default users don't have any boards, so addBoard is the only way for a user to add a board.
        /// </summary>
        /// <param name="toAddTitle">a title</param>
        /// <param name="userEmail">the email of the user</param>
        /// <returns>a json string for adding a board</returns>
        public string addBoard(string toAddTitle, string userEmail)
        {
            try
            {
                log.Info("Attempting to add board " + toAddTitle + " to the user");

                bm.addBoard(toAddTitle, userEmail);
                log.Debug("Added board " + toAddTitle + " successfully");
                Response<string> r = new Response<string>(null, null);
                return JsonController.Serialize(r);

            }
            catch (Exception e)
            {
                log.Error("Board " + toAddTitle + " was not added due to error: " + e.Message);

                Response<string> r = new Response<string>(e.Message, null);
                string json = JsonController.Serialize(r);
                return json;
            }
        }


        /// <summary>
        ///This method is responsible for functional requirement 11, allowing only the owner of a board to delete a board and all its associated tasks
        /// </summary>
        /// <param name="userEmail">an email</param>
        /// <param name="toRemove">the board we want to remove</param>
        /// <returns>a json string for removing a board</returns>
        public string removeBoard(string userEmail, string toRemove)
        {
            try
            {
                log.Info("Attempting to remove board " + toRemove + " from the user");

                bm.removeBoard(toRemove, userEmail);
                log.Debug("Removed board " + toRemove + " successfully");
                Response<string> r = new Response<string>(null, null);
                return JsonController.Serialize(r);

            }
            catch (Exception e)
            {
                log.Error("Board " + toRemove + " was not remove successfuly due to an error: " + e.Message);

                Response<string> r = new Response<string>(e.Message, null);
                string json = JsonController.Serialize(r);
                return json;
            }

        }


        /// <summary>
        ///This method is responsible for the functional requirement 18, allowing any of the board members to add a task to the board.
        ///The new task is added to the backlogcolumn only.
        /// </summary>
        /// <param name="userEmail">the email of the user</param>
        /// <param name="boardName">the name of the board</param>
        /// <param name="title">title of the task</param>
        /// <param name="description">description of the task</param>
        /// <param name="dueDate">duedate of the task</param>
        /// <returns>a json string for adding a task</returns>
        public string addTask(string userEmail, string boardName, string title, string description, DateTime dueDate)
        {
            try
            {

                log.Info("Attempting to add task " + title + " to the board " + boardName);

                bm.addTask(userEmail, boardName, title, description, dueDate);
                log.Debug("Added task " + title + " to board " + boardName + " successfully");
                Response<string> r = new Response<string>(null, null);
                string json = JsonController.Serialize(r);
                return json;

            }
            catch (Exception e)
            {
                log.Error("Task " + title + " wasn't added to board " + boardName + " successfully due to error: " + e.Message);

                Response<string> r = new Response<string>(e.Message, null);
                string json = JsonController.Serialize(r);
                return json;
            }
        }


        /// <summary>
        ///This method is responsible for the functional requirement 19, allowing the assignee of a task to move it from one column to the other.
        ///In the case where the task is found in the backlog, we move the task to inprogress
        ///In the case where the task is found in the inprogress, we move the task to done
        ///In the case where the task is found in done, it can't be moved!
        /// </summary>
        /// <param name="userEmail">the email of the user</param>
        /// <param name="boardName">the name of the board</param>
        /// <param name="columnOrdinal">index of rhe column</param>
        /// <param name="taskId">an id task</param>
        /// <returns>a json string for moving a task</returns>
        public string moveTask(string userEmail, string boardName, int columnOrdinal, int taskId)
        {
            try
            {
                log.Info("Attempting to move task with ID " + taskId + " from the column " + columnOrdinal);

                bm.moveTask(userEmail, boardName, columnOrdinal, taskId);

                log.Debug("Moved task with ID " + taskId + " successfully from the column " + columnOrdinal);

                Response<string> r = new Response<string>(null, null);
                return JsonController.Serialize(r);

            }
            catch (Exception e)
            {
                log.Error("Task with ID " + taskId + " wasn't moved successfully due to error: " + e.Message);

                Response<string> r = new Response<string>(e.Message, null);
                string json = JsonController.Serialize(r);
                return json;
            }
        }


        /// <summary>
        ///This method is responsible for the functional requirement 20 and 21, where a task can be edited only by its assignee.
        ///A task's title, description or duedate can be edited, its creationDate can't
        /// </summary>
        /// <param name="userEmail">the email of the user</param>
        /// <param name="boardName">the name of the board</param>
        /// <param name="columnOrdinal">an index of the column</param>
        /// <param name="taskId">id of a task</param>
        /// <param name="newTitle">a title of a task</param>
        /// <param name="newDesc">a description of a task</param>
        /// <param name="newDueDate">a duedate of a task</param>
        /// <param name="numAct">a number representing variable to edit: 0->title, 1->desc, 2->dueDate.</param>
        /// <returns>a json string for editing a task</returns>
        public string editTask(string userEmail, string boardName, int columnOrdinal, int taskId, string? newTitle, string? newDesc, DateTime? newDueDate, int numAct)
        {
            try
            {
                log.Info("Attempting to edit task with ID " + taskId + " in board " + boardName);

                bm.editTask(userEmail, boardName, columnOrdinal, taskId, newTitle, newDesc, newDueDate, numAct);

                log.Debug("Edited task with ID " + taskId + " successfully");

                Response<string> r = new Response<string>(null, null);
                return JsonController.Serialize(r);

            }
            catch (Exception e)
            {
                log.Error("Task with ID " + taskId + " wasn't edited successfully due to error: " + e.Message);

                Response<string> r = new Response<string>(e.Message, null);
                string json = JsonController.Serialize(r);
                return json;
            }
        }


        /// <summary>
        /// This method is responsible for the functional requirement 16 and 17, a board column can be restricted in size by a specified capacity.
        /// By default, all columns have no limit to the ammount of tasks.
        /// </summary>
        /// <param name="userEmail">the email of the user</param>
        /// <param name="boardName">the name of the board</param>
        /// <param name="limit">the limit</param>
        /// <param name="columnIndex">a column index</param>
        /// <returns>returns a json string for setting a capacity</returns>
        public string setCapacity(string userEmail, string boardName, int limit, int columnIndex)
        {
            try
            {
                log.Info("Attempting to set capacity of column " + columnIndex + " with new capacity " + limit + " in board " + boardName);

                bm.setCapacity(userEmail, boardName, limit, columnIndex);

                log.Debug("Capacity of column " + columnIndex + " successfully set to " + limit);

                Response<string> r = new Response<string>(null, null);
                return JsonController.Serialize(r);

            }
            catch (Exception e)
            {
                log.Error("Column " + columnIndex + "'s capacity wasn't successfully changed to " + limit + " due to error: " + e.Message);

                Response<string> r = new Response<string>(e.Message, null);
                string json = JsonController.Serialize(r);
                return json;
            }

        }


        /// <summary>
        /// This method assists in the fuctional requirement 16 and 17, getting the capacity of a specified column in a specified board.
        /// </summary>
        /// <param name="userEmail">the email of the user</param>
        /// <param name="boardName">the name of the board</param>
        /// <param name="columnIndex">a column index</param>
        /// <returns>a json string for getting a capacity</returns>
        public string getCapacity(string userEmail, string boardName, int columnIndex)
        {
            try
            {
                log.Info("Attempting to get capacity of " + columnIndex + " from board " + boardName);

                int capacity = bm.getCapacity(userEmail, boardName, columnIndex);

                log.Debug("Capacity of column " + columnIndex + " from board " + boardName + " was successfully returned");

                ResponseInt r = new ResponseInt(capacity);
                string json = JsonController.Serialize(r);
                return json;

            }
            catch (Exception e)
            {
                log.Error("Column with index " + columnIndex + "'s capacity wasn't successfully returned due to error: " + e.Message);
                Response<string> r = new Response<string>(e.Message, null);
                string json = JsonController.Serialize(r);
                return json;
            }
        }


        /// <summary>
        /// This method gets the name of a specified columnIndex of a specified board.
        /// </summary>
        /// <param name="userEmail">the email of the user</param>
        /// <param name="boardName">the name of the board</param>
        /// <param name="columnIndex">the index of the column</param>
        /// <returns>a json string for getting a column name</returns>
        public string getColumnName(string userEmail, string boardName, int columnIndex)
        {
            try
            {
                log.Info("Attempting to get name of column " + columnIndex + " from board " + boardName);

                string colname = bm.getColumnName(userEmail, boardName, columnIndex);

                log.Debug("Name of column " + columnIndex + " from board " + boardName + " was successfully returned");

                Response<string> r = new Response<string>(null, colname);
                string json = JsonController.Serialize(r);
                return json;

            }
            catch (Exception e)
            {
                log.Error("Column " + columnIndex + "'s name wasn't returned successfully from board " + boardName + " due to error: " + e.Message);

                Response<string> r = new Response<string>(e.Message, null);
                string json = JsonController.Serialize(r);
                return json;
            }
        }


        /// <summary>
        ///This method is responsible for the functional requirement 22, allowing a user to list all assigned tasks to the user from all boards that are found in the 
        ///"InProgressList", allowing the user to create their own timeline or calendar to complete said tasks.
        /// </summary>
        /// <param name="client">a client</param>
        /// <returns>a json string for listing the in-progress tasks</returns>
        public string ListProgressTasks(string client)
        {
            try
            {
                log.Info("Attempting to list ProgressTasks of the user");

                List<BusinessLayer.Task> list = bm.listProgressTasks(client);

                log.Debug("ProgressTasks were successfully returned");


                Object[] array = new object[list.Count];
                for (int i = 0; i < array.Length; i++)
                    array[i] = list[i];

                Response<Object[]> r = new Response<Object[]>(null, array);
                string json = JsonController.Serialize(r);
                return json;

            }
            catch (Exception e)
            {
                log.Error("The progress tasks of the user were not successfully listed due to error: " + e.Message);

                Response<string> r = new Response<string>(e.Message, null);
                string json = JsonController.Serialize(r);
                return json;
            }

        }


        /// <summary>
        ///This method returns all tasks in a specified column
        /// </summary>
        /// <param name="email">an email</param>
        /// <param name="boardName">the name of the board</param>
        /// <param name="columnOrdinal">an index of the column</param>
        /// <returns>a json string for getting a column</returns>
        public string GetColumn(string email, string boardName, int columnOrdinal)
        {
            try
            {
                log.Info("Attempting to get column " + columnOrdinal + " from board " + boardName);

                List<BusinessLayer.Task> list = bm.GetColumn(email, boardName, columnOrdinal);

                log.Debug("Column " + columnOrdinal + " was successfully returned from board " + boardName);

                Object[] array = new object[list.Count];
                for (int i = 0; i < array.Length; i++)
                    array[i] = list[i];

                Response<Object[]> r = new Response<Object[]>(null, array);
                string json = JsonController.Serialize(r);
                return json;

            }
            catch (Exception e)
            {
                log.Error("Column with ID " + columnOrdinal + " wasn't successfully returned from board " + boardName + " due to error: " + e.Message);

                Response<string> r = new Response<string>(e.Message, null);
                string json = JsonController.Serialize(r);
                return json;
            }

        }


        /// <summary>
        /// this method returns the boards of the user
        /// </summary>
        /// <param name="email">the email of the user</param>
        /// <returns>json string of the boards of the user</returns>
        public string GetUserBoards(string email)
        {
            try
            {
                log.Info("Attempting to get IDs of all boards user is a member of.");

                List<int> boardIDs = bm.GetUserBoards(email);

                log.Debug("All boards of user were successfully returned!");

                Object[] array = new object[boardIDs.Count];
                for (int i = 0; i < array.Length; i++)
                    array[i] = boardIDs[i];

                Response<Object[]> r = new Response<Object[]>(null, array);
                string json = JsonController.Serialize(r);
                return json;
            }
            catch (Exception e)
            {

                log.Error("Boards of user were not successfully listed due to error: " + e.Message);

                Response<string> r = new Response<string>(e.Message, null);
                string json = JsonController.Serialize(r);
                return json;
            }
        }

        /// <summary>
        /// This method is responsible for listing the boards of the users
        /// </summary>
        /// <param name="email">the email of the user</param>
        /// <returns>a json of the list of the users</returns>
        public string getUsersBoards(string email)
        {

            try
            {
                log.Info("Attempting to get boards of user.");

                List<Board> boards = bm.getUsersBoards(email);

                log.Debug("All boards of user were successfully returned!");

                Response<List<Board>> r = new Response<List<Board>>(null, boards);
                string json = JsonController.Serialize(r);
                return json;
            }
            catch (Exception e)
            {

                log.Error("Boards of user were not successfully listed due to error: " + e.Message);

                Response<string> r = new Response<string>(e.Message, null);
                string json = JsonController.Serialize(r);
                return json;
            }
        }

        /// <summary>
        /// This method is responsible for the functional requirement 12, allowing a user to join a board as a member
        /// </summary>
        /// <param name="userEmail">the email of the user that wants to join a board as a member</param>
        /// <param name="boardId">the board that said user wants to join</param>
        /// <returns>json string of a user joinnig a board</returns>
        public string JoinBoard(string userEmail, int boardId)
        {
            try
            {
                log.Info("Attempting to add user to board " + boardId + " as a member");

                bm.JoinBoard(userEmail, boardId);

                log.Debug("Adding user to board " + boardId + " as a member was successful");

                Response<string> r = new Response<string>(null, null);
                return JsonController.Serialize(r);
            }
            catch (Exception e)
            {
                log.Error("User was unable to join board " + boardId + " due to error: " + e.Message);

                Response<string> r = new Response<string>(e.Message, null);
                string json = JsonController.Serialize(r);
                return json;
            }
        }


        /// <summary>
        /// This method is responsible for the functional requirement 12, 14 and 15, where only members are allowed to leave a board (owners are not allowed to leave).
        /// Once a member leaves a board, all their assigned tasks automatically become unassigned
        /// </summary>
        /// <param name="userEmail">the email of the user that wants to leave (can only be a member of the board, can not be the owner).</param>
        /// <param name="boardId">the Id of the board that the user wants to leave</param>
        /// <returns>json string of a member of the board leaving the board member list</returns>
        public string LeaveBoard(string userEmail, int boardId)
        {
            try
            {
                log.Info("Attempting to remove user from board " + boardId);

                bm.LeaveBoard(userEmail, boardId);

                log.Debug("Removing user from board " + boardId + " was successful");

                Response<string> r = new Response<string>(null, null);
                return JsonController.Serialize(r);
            }
            catch (Exception e)
            {
                log.Error("User was unable to be removed from board " + boardId + " due to error: " + e.Message);

                Response<string> r = new Response<string>(e.Message, null);
                string json = JsonController.Serialize(r);
                return json;
            }
        }


        /// <summary>
        /// The method changes the assignee of the task, it is responsible for the functional requirement 23.
        /// </summary>
        /// <param name="email">the email of the assigner</param>
        /// <param name="boardName">the name of the board</param>
        /// <param name="columnOrdinal">the id of the column</param>
        /// <param name="taskId">id of the task</param>
        /// <param name="emailAssignee">email of the assignee</param>
        /// <returns>an empty json string </returns>
        public string AssignTask(string email, string boardName, int columnOrdinal, int taskId, string emailAssignee)
        {
            try
            {
                log.Info("Attempting to assign new user to task " + taskId + " in board " + boardName);

                bm.assignTask(email, boardName, columnOrdinal, taskId, emailAssignee);

                log.Debug("Assigning new user to task " + taskId + " from board " + boardName + " was successful");

                Response<string> r = new Response<string>(null, null);
                return JsonController.Serialize(r);
            }
            catch (Exception e)
            {
                log.Error("Assigning task " + taskId + "in board " + boardName + " with new assignee was unsuccessful due to error: " + e.Message);

                Response<string> r = new Response<string>(e.Message, null);
                string json = JsonController.Serialize(r);
                return json;
            }
        }


        /// <summary>
        /// getter method for a board name
        /// </summary>
        /// <param name="boardId">id of the board</param>
        /// <returns>a json string of the board name</returns>
        public string GetBoardName(int boardId)
        {
            try
            {
                log.Info("Attempting to get name of board " + boardId);

                string boardName = bm.getBoardName(boardId);

                log.Debug("Name of board " + boardId + " was successfully returned");

                Response<string> r = new Response<string>(null, boardName);
                string json = JsonController.Serialize(r);
                return json;
            }
            catch (Exception e)
            {
                log.Error("Name of board " + boardId + " was unsuccessfully retrieved due to error: " + e.Message);

                Response<string> r = new Response<string>(e.Message, null);
                string json = JsonController.Serialize(r);
                return json;
            }
        }


        /// <summary>
        /// This method is responsible for functional requirement 13, and lets a board owner transfer the board ownership to someone else. 
        /// </summary>
        /// <param name="currentOwnerEmail">the email of the transfering owner</param>
        /// <param name="newOwnerEmail">the email of the new owner</param>
        /// <param name="boardName">the name of the board</param>
        /// <returns>an empty json string</returns>
        public string TransferOwnership(string currentOwnerEmail, string newOwnerEmail, string boardName)
        {
            try
            {
                log.Info("Attempting to transfer ownership of board " + boardName);

                bm.transferOwner(currentOwnerEmail, newOwnerEmail, boardName);

                log.Debug("Transferring ownership of board " + boardName + " was successful");

                Response<string> r = new Response<string>(null, null);
                return JsonController.Serialize(r);
            }
            catch (Exception e)
            {
                log.Error("Transferring owner of board " + boardName + " was unsuccessfully transferred due to error: " + e.Message);

                Response<string> r = new Response<string>(e.Message, null);
                string json = JsonController.Serialize(r);
                return json;
            }
        }

        /// <summary>
        /// This function loads data from the database into our project
        /// </summary>
        /// <returns>an empty json string</returns>
        public string LoadData()
        {
            try
            {
                log.Info("Attempting to load data from database");

                bm.LoadData();

                log.Debug("loading data from database was successful");

                Response<string> r = new Response<string>(null, null);
                return JsonController.Serialize(r);
            }
            catch (Exception e)
            {
                log.Error("Loading data was unsuccessful due to error: " + e.Message);

                Response<string> r = new Response<string>(e.Message, null);
                string json = JsonController.Serialize(r);
                return json;
            }
        }

        /// <summary>
        /// This function removes data from our project variables
        /// </summary>
        /// <returns>an empty json string</returns>
        public string RemoveData()
        {
            try
            {
                log.Info("Attempting to remove data from project variables (on RAM)");

                bm.RemoveData();

                log.Debug("removing data was successful");

                Response<string> r = new Response<string>(null, null);
                return JsonController.Serialize(r);
            }
            catch (Exception e)
            {
                log.Error("Removing data from project variables was unsuccessful due to error: " + e.Message);

                Response<string> r = new Response<string>(e.Message, null);
                string json = JsonController.Serialize(r);
                return json;
            }
        }
    }
}