using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using log4net;
using log4net.Config;
using System.IO;
using System.Reflection;
using IntroSE.Kanban.Backend.DataAccessLayer;

namespace IntroSE.Kanban.Backend.BusinessLayer
#nullable enable
{
    public class BoardManagement
    {
        /// <summary>
        /// BoardManagement is a class that is used in order to store, add and delete Boards for specified users. In addition it also 
        /// includes the method for listingallprogresstasks of a user due to BoardManagement having a list of all boards and a list of all users
        /// </summary>
        /// <param name = "boardList" >list of boards</param>
        /// <param name = "um">a user controller</param>
        /// <param name = "boardIdCounter">Counter for a board</param>
        /// <param name = "boardDalController">a controller for a board table</param>
        /// <param name = "userBoardDalController">a controller for a user-board table</param>
        /// <param name = "taskDalController">a controller for a task table</param>
        /// <param name = "BoardIdCounterDalController">a controller for a boardId table</param>
        /// <param name = "taskCounterDalController">a controller for a taskCounter table</param>


        //fields
        private List<Board> boardList;
        private UserManagement um;
        private int boardIdCounter;
        private BoardDalController boardDalController;
        private UserBoardDalController userBoardDalController;
        private TaskDalController taskDalController;
        private BoardIdCounterDalController boardIdCounterDalController;
        private TaskCounterDalController taskCounterDalController;
        private readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);


        //constructor
        public BoardManagement(UserManagement um)
        {
            BoardList = new List<Board>();
            Um = um;
            BoardIdCounter = 0;
            boardDalController = new BoardDalController();
            userBoardDalController = new UserBoardDalController();
            taskDalController = new TaskDalController();
            boardIdCounterDalController = new BoardIdCounterDalController();
            taskCounterDalController = new TaskCounterDalController();



            //logging
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));

        }


        //getters and setters
        public List<Board> BoardList { get { return boardList; } set { boardList = value; } }
        public UserManagement Um { get { return um; } set { um = value; } }
        public int BoardIdCounter { get { return boardIdCounter; } set { boardIdCounter = value; } }
        public BoardDalController BoardDalController { get { return boardDalController; } set { boardDalController = value; } }
        public TaskDalController TaskDalController { get { return taskDalController; } set { taskDalController = value; } }
        public UserBoardDalController UserBoardDalController { get { return userBoardDalController; } set { userBoardDalController = value; } }
        public BoardIdCounterDalController BoardIdCounterDalController { get { return boardIdCounterDalController; } set { boardIdCounterDalController = value; } }
        public TaskCounterDalController TaskCounterDalController { get { return taskCounterDalController; } set { taskCounterDalController = value; } }




        //methods

        /// <summary>
        /// getting board of a user with a specified name
        /// </summary>
        /// <param name="user">a user</param>
        /// <param name="boardName">the name of the board</param>
        /// <returns>A board</returns>
        /// <exception cref="Exception">when boardname is null/doesnt exists or when user isnt logged out</exception>
        public Board getBoard(User user, string boardName)
        {
            log.Debug("method getBoard was successfully called in BoardManagement, attempting to get board " + boardName);
            um.checks(user.Email);//validates user being in the system

            if (boardName == null)
                throw new Exception("Board name is null");

            if (user.LoggedIn)
            {
                foreach (Board board in boardList)
                {
                    if (board.boardName.Equals(boardName) && board.isMember(user.Email))
                    {
                        return board;
                    }
                }
                throw new Exception("Board does not exist or user is not member of board");
            }
            throw new Exception("User is not logged in");
        }

        /// <summary>
        /// A getter method
        /// </summary>
        /// /// <param name="id">an id of a board</param>
        /// <returns>A board</returns>
        public Board getBoard(int id)
        {
            log.Debug("Successfully called method getBoard in BoardManagement, attempting to retrieve board " + id);

            foreach(Board board in boardList)
                if(board.BoardId == id)
                    return board;

            throw new Exception("Board does not exist");
        }


        /// <summary>
        /// Following method finds a board with a specified ID
        /// </summary>
        /// <param name="boardId">the id of the board</param>
        /// <returns>the board itself from the boardList</returns>
        /// <exception cref="Exception">board with such ID does not exist</exception>
        public string getBoardName(int boardId)
        {
            log.Debug("Successfully called method getBoardName in BoardManagement, attempting to retrieve board name of board " + boardId);

            foreach(Board board in boardList)
            {
                if (board.BoardId.Equals(boardId))
                {
                    return board.BoardName;
                }
            }

            throw new Exception("Board does not exist with such ID");
        }


        /// <summary>
        /// Getting column Name of a specified column in a board
        /// </summary>
        /// <param name="userEmail">the email of the user</param>
        /// <param name="boardName">the name of the board</param>
        /// <param name="column">column index</param>
        /// <returns>a string value of a column name</returns>
        /// <exception cref="Exception">when board name is null or when user isnt logged in or when column number doesnt match to anything</exception>
        public string getColumnName(string userEmail, string boardName, int column)
        {
            log.Debug("successfully called method getColumnName in BoardManagement, attempting to get column " + column + " in board " + boardName);

            um.checks(userEmail);
            userEmail = userEmail.ToLower();

            if (boardName == null)
                throw new Exception("Board name is null");

            if (um.isLoggedIn(userEmail) & boardExists(boardName, userEmail))
            {
                if (column == 0)
                    return "backlog";
                if (column == 1)
                    return "in progress";
                if (column == 2)
                    return "done";

                throw new Exception("Column number does not match to any of the exist columns");
            }

            throw new Exception("User is not logged in");
        }


        /// <summary>
        /// Getting capacity of a specified column in a specified board of a user
        /// </summary>
        /// <param name="userEmail">the email of the user</param>
        /// <param name="boardName">the name of the board</param>
        /// <param name="columnIndex">a column index</param>
        /// <returns>the capacity</returns>
        /// <exception cref="Exception">when board name is null or when user isnt logged in</exception>
        public int getCapacity(string userEmail, string boardName, int columnIndex)
        {
            log.Debug("successfully called method getCapacity in BoardManagement, attempting to get capacity of column " + columnIndex + " in board " + boardName);

            um.checks(userEmail);
            userEmail = userEmail.ToLower();

            if (boardName == null)
                throw new Exception("Board name is null");

            if (um.isLoggedIn(userEmail))
            {
                User user = um.GetUser(userEmail);
                Board board = getBoard(user, boardName);
                return board.getCapacity(columnIndex);
            }

            throw new Exception("User not logged in");
        }


        /// <summary>
        /// getting a specified column of a specified board in a user
        /// </summary>
        /// <param name="email">an email</param>
        /// <param name="boardName">the name of the board</param>
        /// <param name="columnOrdinal">index of the column</param>
        /// <returns>a list of tasks</returns>
        /// <exception cref="Exception">when board name is null or when user isnt logged in</exception>
        public List<Task> GetColumn(string email, string boardName, int columnOrdinal)
        {
            log.Debug("successfully called method getColumn in BoardManagement, attempting to get column " + columnOrdinal + " from board " + boardName);

            um.checks(email);
            email = email.ToLower();

            if (boardName == null)
                throw new Exception("Board name is null");

            if (um.isLoggedIn(email))
            {
                User user = um.GetUser(email);
                Board board = getBoard(user, boardName);

                return board.getColumn(columnOrdinal);

            }

            throw new Exception("User is not logged in");
        }


        /// <summary>
        /// Setting a new capacity on a specified column in a specified board of a user 
        /// </summary>
        /// <param name="userEmail">the email of the user</param>
        /// <param name="boardName">the name of the board</param>
        /// <param name="limit">a limit</param>
        /// <param name="columnIndex">index of the column</param>
        /// limit ==> =-1 > unlimit  
        /// <exception cref="Exception">when board name is null or when user isnt logged in</exception>
        public void setCapacity(string userEmail, string boardName, int limit, int columnIndex)
        {
            log.Debug("successfully called method setCapacity in BoardManagement, attempting to set capacity of " + columnIndex + "to " + limit + " in board " + boardName);

            um.checks(userEmail);
            userEmail = userEmail.ToLower();

            if (boardName == null)
                throw new Exception("Board name is null");

            if (um.isLoggedIn(userEmail))
            {
                User user = um.GetUser(userEmail);
                Board board = getBoard(user, boardName);
                board.setCapacity(limit, columnIndex,boardDalController);
            }
            else
            {
                throw new Exception("User is not logged in");
            }
        }


        /// <summary>
        ///This function adds a board, according to functional requirerment 9. This is done by first validating the user being logged in,
        ///as well as ensuring that the board doesnt already exist in our system (since a user can't have the same board multiple times)
        /// </summary>
        /// <param name="toAddTitle">a title</param>
        /// <param name="userEmail">the email of the user</param>
        /// <exception cref="Exception">board name is null or when user isnt logged in or when board already exists</exception>
        public void addBoard(string toAddTitle, string userEmail)
        {
            log.Debug("successfully called method addBoard in BoardManagement, attempting to add board " + toAddTitle + " to user.");

            um.checks(userEmail); //validates user being in the system
            userEmail = userEmail.ToLower();

            if (!boardCheck(toAddTitle))
            {
                throw new Exception("Board name is Illegal");
            }

            if (um.isLoggedIn(userEmail))
            { //only a logged in user can add boards

                if (!boardExists(toAddTitle, userEmail))
                {
                    if (boardDalController.Insert(new BoardDTO(toAddTitle, userEmail, boardIdCounter, -1, -1, -1)))
                    {
                        if (userBoardDalController.Insert(new UserBoardDTO(userEmail, boardIdCounter, "yes")))
                        {

                            if (boardIdCounter == 0)
                            {
                                if (!BoardIdCounterDalController.Insert(new BoardIdCounterDTO(1)))
                                {
                                    userBoardDalController.Delete(boardIdCounter);
                                    boardDalController.Delete(boardIdCounter);
                                    throw new Exception("Database failed to insert new Board");
                                }
                            }

                            if (taskCounterDalController.Insert(new TaskIdCounterDTO(BoardIdCounter, 0)))
                            {
                                Board toAdd = new Board(toAddTitle, userEmail, BoardIdCounter);
                                boardList.Add(toAdd);
                                boardIdCounter++;

                                if (!boardIdCounterDalController.Update("counter", BoardIdCounter)) 
                                {
                                    BoardIdCounter--;
                                    boardList.Remove(toAdd);
                                    taskCounterDalController.Delete(BoardIdCounter);
                                    userBoardDalController.Delete(BoardIdCounter);
                                    boardDalController.Delete(BoardIdCounter);
                                }

                            }
                            else
                            {
                                userBoardDalController.Delete(boardIdCounter);
                                boardDalController.Delete(boardIdCounter);
                                throw new Exception("Database failed to insert new Board");
                            }
                        }
                        else
                        {
                            boardDalController.Delete(boardIdCounter);
                            throw new Exception("Database failed to insert new Board");
                        }
                    }
                    else
                    {
                        throw new Exception("Database failed to insert new Board");
                    }
                }
                else
                {
                    throw new Exception("Board already exists");
                }
            }
            else
            {
                throw new Exception("user isnt logged in");
            }

        }


        /// <summary>
        /// This method checks if the boards' name is legal
        /// </summary>
        /// <param name="boardName">the name of the board</param>
        /// <returns>returns true if the name is valid, otherwise returns false</returns>
        public bool boardCheck(string boardName)
        {
            log.Debug("Successfully called method boardCheck in BoardManagement, attempting to check if board name is legal");

            if (boardName == null)
                return false;

            if (boardName.Length == 0)
                return false;

            for (int i = 0; i < boardName.Length; i++)
            {
                if (boardName[i] != ' ')
                {
                    return true;
                }
            }

            return false;
        }


        /// <summary>
        ///This function removes a board, according to functional requirerment 9, this is done so by first ensuring the user
        ///is registered in the system, the user is logged in and that the board is existant in our system, as well as 
        ///associated to said user.
        /// </summary>
        /// <param name="boardNameToRemove">the board we want to remove</param>
        /// <param name="userEmail">the email of the user</param>
        /// <exception cref="Exception">when board name is null or when user isnt logged in</exception>
        public void removeBoard(string boardNameToRemove, string userEmail)
        {
            log.Debug("successfully called method removeBoard in BoardManagement, attempting to remove board " + boardNameToRemove);

            um.checks(userEmail); //validates user being in the system
            userEmail = userEmail.ToLower();

            if (boardNameToRemove == null)
            {
                throw new Exception("Board name is null");
            }

            if (um.isLoggedIn(userEmail))//only a loggedIn user can remove boards
            {
                User user = um.GetUser(userEmail);
                Board toRemove = getBoard(user, boardNameToRemove);//verifies the board being in the list

                if (toRemove.Owner.Equals(userEmail))
                {
                    if (boardDalController.Delete(toRemove.BoardId))
                    {
                        if (userBoardDalController.Delete(toRemove.BoardId))
                        {
                            if (taskDalController.Delete(toRemove.BoardId))
                            {
                                if (taskCounterDalController.Delete(toRemove.BoardId))
                                {
                                    boardList.Remove(toRemove);
                                }
                                else
                                {

                                    foreach(Task task in toRemove.BacklogList)
                                    {
                                        taskDalController.Insert(new TaskDTO(task.TaskId, task.Title, task.Description,task.CreationTime, task.DueDate, task.BoardId, 0, task.Assignee));
                                    }

                                    foreach (Task task in toRemove.InProgressList)
                                    {
                                        taskDalController.Insert(new TaskDTO(task.TaskId, task.Title, task.Description, task.CreationTime, task.DueDate, task.BoardId, 0, task.Assignee));
                                    }

                                    foreach (Task task in toRemove.DoneList)
                                    {
                                        taskDalController.Insert(new TaskDTO(task.TaskId, task.Title, task.Description, task.CreationTime, task.DueDate, task.BoardId, 0, task.Assignee));
                                    }


                                    foreach (String email in toRemove.Collaborators)
                                    {
                                        if (toRemove.Owner.Equals(email))
                                        { 
                                            userBoardDalController.Insert(new UserBoardDTO(email, toRemove.BoardId, "yes"));
                                        }
                                        else
                                        {
                                            userBoardDalController.Insert(new UserBoardDTO(email, toRemove.BoardId, "no"));

                                        }
                                    }
                                    boardDalController.Insert(new BoardDTO(toRemove.boardName, toRemove.Owner, toRemove.BoardId, toRemove.Capacities[0], toRemove.Capacities[1], toRemove.Capacities[2]));
                                    throw new Exception("Database failed to remove board");

                                }
                            }
                            else
                            {
                                foreach (String email in toRemove.Collaborators)
                                {
                                    if (toRemove.Owner.Equals(email))
                                    {
                                        userBoardDalController.Insert(new UserBoardDTO(email, toRemove.BoardId, "yes"));
                                    }
                                    else
                                    {
                                        userBoardDalController.Insert(new UserBoardDTO(email, toRemove.BoardId, "no"));

                                    }
                                }
                                boardDalController.Insert(new BoardDTO(toRemove.boardName, toRemove.Owner, toRemove.BoardId, toRemove.Capacities[0], toRemove.Capacities[1], toRemove.Capacities[2]));

                                throw new Exception("Database failed to remove board");

                            }

                        }
                        else
                        {
                            boardDalController.Insert(new BoardDTO(toRemove.boardName, toRemove.Owner, toRemove.BoardId, toRemove.Capacities[0], toRemove.Capacities[1], toRemove.Capacities[2]));
                            throw new Exception("Database failed to remove board");

                        }
                    }
                    else
                        throw new Exception("Database failed to remove board");
                }
                else
                {
                    throw new Exception("User is not owner of board");
                }
                
            }
            else
            {
                throw new Exception("user isnt logged in");
            }

        }


        /// <summary>
        ///This function checks if the board exists, used in other methods such as removeBoard and addBoard, ensuring the integrity rule #6
        /// </summary>
        /// <param name="boardName">the name of the board</param>
        /// <param name="userEmail">the email of the user</param>
        /// <returns>if the board exists</returns>
        /// <exception cref="Exception">when board name is null or when user isnt logged in</exception>
        public bool boardExists(string boardName, string userEmail)
        {
            log.Debug("successfully called method boardExists in BoardManagement, checking if board " + boardName + " exists");

            um.checks(userEmail); //validates user being in the system
            userEmail = userEmail.ToLower();

            if (boardName == null)
            {
                throw new Exception("Board name is null");
            }
            if (um.isLoggedIn(userEmail))
            {
                foreach (Board board in boardList)
                {
                    if (board.BoardName.Equals(boardName) && board.isMember(userEmail))
                    {
                        return true;
                    }
                }
                return false;
            }

            throw new Exception("User is not logged in");
        }


        /// <summary>
        /// This method is responsible for checking if the board exists in the user boards.
        /// </summary>
        /// <param name="boardName">the name of the board</param>
        /// <param name="userEmail">the email pf the user</param>
        /// <returns>returns true if the board exists and else otherwise</returns>
        /// <exception cref="Exception">throws exception when the board is null</exception>
        public bool boardExistsLoad(string boardName, string userEmail)
        {
            log.Debug("successfully called method boardExists in BoardManagement, checking if board " + boardName + " exists");

            um.checks(userEmail); //validates user being in the system
            userEmail = userEmail.ToLower();

            if (boardName == null)
            {
                throw new Exception("Board name is null");
            }
            
            foreach (Board board in boardList)
            {
                if (board.BoardName.Equals(boardName) && board.isMember(userEmail))
                {
                    return true;
                }
            }
            return false;
            
        }


        /// <summary>
        ///This function adds a task to the backloglist, according to functional requirerment 12, by first ensuring the capacity and then adding 
        ///if the capacity allows for such
        /// </summary>
        /// <param name="userEmail"> the email of the user</param>
        /// <param name="boardName"> the name of the board we want to add a task</param>
        /// <param name="title"> the title of the task we want to create and add</param>
        /// <param name="description"> the description of the task we want to create and add</param>
        /// <param name="dueDate">the duedate of the task we want to create create and add</param>
        /// <exception cref="Exception">when board name is null or when user isnt logged in</exception>
        public Task addTask(string userEmail, string boardName, string title, string description, DateTime dueDate)
        {
            log.Debug("successfully called method addTask in BoardManagement, attempting to add the task " + title);

            um.checks(userEmail);
            userEmail = userEmail.ToLower();

            if (boardName == null)
                throw new Exception("Board name is null");

            if (um.isLoggedIn(userEmail))
            {
                User user = um.GetUser(userEmail);
                Board board = getBoard(user, boardName);

                Task task = board.addTask(title, description, dueDate,taskDalController);
                TaskIdCounterDTO taskIdCounterDTO = new TaskIdCounterDTO(board.BoardId, task.TaskId);
                taskIdCounterDTO.Counter++;
                return task;
                
            }
            else
            {
                throw new Exception("user is not logged in");
            }

        }


        /// <summary>
        /// This function moves a task, according to functional requirerment 13, this is done in a few steps:
        ///a. find the location of task, in one of the lists or not even found at all
        ///b. if in backLogList, ensure that inProgressList has a sufficient capacity size for adding
        ///c. if in progressList, ensure that the doneList has a sufficient capacity size for adding
        ///d. if in doneList or not found, can't be moved to anywhere
        /// </summary>
        /// <param name="userEmail"> the email of the user</param>
        /// <param name="boardName"> the name of the board we want to move the task</param>
        /// <param name="columnOrdinal">the index of the column</param>
        /// <param name="taskId"> the id of the task</param>
        /// <exception cref="Exception">when board name is null or when user isnt logged in</exception>
        public void moveTask(string userEmail, string boardName, int columnOrdinal, int taskId)
        {
            log.Debug("successfully called method moveTask in BoardManagement, attempting to move the task " + taskId + " to the next column");

            um.checks(userEmail);
            userEmail = userEmail.ToLower();

            if (boardName == null)
                throw new Exception("Board name is null");

            if (um.isLoggedIn(userEmail))
            {
                User user = um.GetUser(userEmail);
                Board board = getBoard(user, boardName);
                
                board.moveTask(columnOrdinal, taskId, userEmail,taskDalController);
            }
            else
            {
                throw new Exception("user is not logged in");
            }
        }


        /// <summary>
        /// This function edits the editTitle method, according to functional requirerment 15. 
        /// We make sure that task is not in "done" state- according to functional requirerment 14.
        /// </summary>
        /// <param name="userEmail"> the email of the user we want to edit a task</param>
        /// <param name="boardName"> the name of the board where the task we want to edit is found</param>
        /// <param name="columnOrdinal">the index of the column</param>
        /// <param name="taskId"> the id of the task we want to edit</param>
        /// <param name="newTitle">the possible new title we want to change to</param>
        /// <param name="newDesc"> the possible new description we want to change to</param>
        /// <param name="newDueDate"> the possible new duedate we want to change to</param>
        /// <param name="numAct">   </param>
        /// <exception cref="Exception">when board name is null or when user isnt logged in</exception>
        public void editTask(string userEmail, string boardName, int columnOrdinal, int taskId, string? newTitle, string? newDesc, DateTime? newDueDate, int numAct)
        {
            log.Debug("successfully called method editTask in BoardManagement, attempting to edit task " + taskId);

            um.checks(userEmail);
            userEmail = userEmail.ToLower();

            if (boardName == null)
                throw new Exception("Board name is null");

            if (um.isLoggedIn(userEmail))
            {
                User user = um.GetUser(userEmail);
                Board board = getBoard(user, boardName);

                board.editTask(columnOrdinal, taskId, newTitle, newDesc, newDueDate, numAct, userEmail,taskDalController);
            }

            else
            {
                throw new Exception("user is not logged in");
            }
        }


        ///<summary>
        ///This function lists all of the tasks that are in progress state, according to functional requirerment 16:
        ///it goes over the entire list of boards, 
        ///finds the boards associated with requested client,
        ///adds the progresstasks of each associated board to one list that we return at the end.
        /// </summary>
        /// <param name="userEmail">the email of the user</param>
        /// <returns>a list of "in-progress" tasks</returns>
        /// <exception cref="Exception">when there are no boards for this user or when user isnt logged in</exception>
        public List<Task> listProgressTasks(string userEmail)
        {
            log.Debug("successfully called method listProgressTasks in BoardManagement, attempting to list the in progress tasks of user");

            um.checks(userEmail);
            userEmail = userEmail.ToLower();

            if (um.isLoggedIn(userEmail))
            { //only a logged in user can list their in progress tasks

                List<Task> toReturn = new List<Task>();

                //iterating over all boards
                foreach (Board board in boardList)
                {
                    if (board.isMember(userEmail))
                    {
                        //adding all elements from progressTasks in the current board to the toReturn
                        List<Task> temp = board.listInProgressTask(userEmail);

                        foreach (Task task in temp)
                        {
                            toReturn.Add(task);
                        }
                    }
                }
                return toReturn;
            }

            throw new Exception("user not logged in");
        }


        /// <summary>
        /// The method is used to list all boards of user
        /// </summary>
        /// <param name="email">email of user</param>
        /// <returns>a list of boards</returns>
        /// <exception cref="Exception"> user is not logged in</exception>
        public List<Board> getUsersBoards(string email)
        {
            log.Debug("Successfully called method GetUsersBoards in BoardManagement, attempting to get boards of user.");

            List<Board> boards = new List<Board>();
            um.checks(email);
            email = email.ToLower();

            if (um.isLoggedIn(email))
            {
                foreach (Board board in BoardList)
                {
                    if (board.isMember(email))
                    {
                        boards.Add(board);
                    }
                }
                return boards;
            }
            else
            {
                throw new Exception("User is not logged in");
            }
        } 

        /// <summary>
        /// this method is responsible for functional requirement 12, and lets a user join a board.
        /// </summary>
        /// <param name="email">the email of a user</param>
        /// <param name="boardId">the id of the board</param>
        /// <returns>returns true if the user joined, or false otherwise</returns>
        public bool JoinBoard(string email, int boardId)
        {
            log.Debug("successfully called method JoinBoard in BoardManagement, attempting to add user to board " + boardId);

            um.checks(email);
            email = email.ToLower();

            User user = um.GetUser(email);
            Board board = getBoard(boardId);
            string boardName = getBoardName(boardId);

            if (um.isLoggedIn(email))
            {
                try
                {
                    Board Exists = getBoard(user, boardName);
                }
                catch (Exception e)
                {
                
                    if (userBoardDalController.Insert(new UserBoardDTO(email, boardId, "no")))
                    {
                        board.addMember(email);
                        return true;
                    }
                    else
                        throw new Exception("Database failed to add board member");
                }
                throw new Exception("User already owns/is a member of a board with same name");
            }
            else
            {
                throw new Exception("User is not logged in");
            }
        }


        /// <summary>
        /// this method is responsible for functional requirement 12, and lets a user leave a board.
        /// </summary>
        /// <param name="email">the email of the user</param>
        /// <param name="boardId">the board id</param>
        /// <returns>returns true if the user left the board, or else otherwise</returns>
        public bool LeaveBoard(string email, int boardId)
        {
            log.Debug("successfully called method LeaveBoard in BoardManagement, attempting to remove user from board " + boardId);

            um.checks(email);
            email = email.ToLower();

            string boardName = getBoardName(boardId);

            User user = um.GetUser(email);
            Board board = getBoard(user,boardName);

            if (um.isLoggedIn(email))
            {
                if (board.Owner.Equals(email))
                {
                    throw new Exception("User is owner of board, cannot leave board");
                }
                if (board.isMember(email))
                {
                    if (userBoardDalController.Delete(email, boardId))
                    {

                        if (board.removeMember(email, taskDalController))
                            return true;
                        else
                        {
                            userBoardDalController.Insert(new UserBoardDTO(email, boardId, "no"));
                            throw new Exception("");
                        }
                    }
                    else
                    {
                        throw new Exception("Database failed to remove board member");
                    }
                }
                else
                {
                    throw new Exception("User is not member");
                }
            }
            else
            {
                throw new Exception("User is not logged in");
            }
        }


        /// <summary>
        /// This method is responsible for functional requirement 13, and lets a board owner transfer the board ownership to someone else. 
        /// </summary>
        /// <param name="owner">the current board owner</param>
        /// <param name="newOwner">the new owner</param>
        /// <param name="boardName">id of the board</param>
        /// <returns>returns true if the transfering succeded, false otherwise,</returns>
        /// <exception cref="Exception">inputs are not valid</exception>
        public bool transferOwner(string owner, string newOwner, string boardName)
        {
            log.Debug("successfully called method transferOwner in BoardManagement, attempting to transfer ownership of board "+ boardName +" to new owner");

            um.checks(owner);
            um.checks(newOwner);

            owner = owner.ToLower();
            newOwner = newOwner.ToLower();

            if (um.isLoggedIn(owner))
            {
                User user = um.GetUser(owner);
                Board board = getBoard(user, boardName);

                if (board.Owner.Equals(owner))
                {
                    if (board.isMember(newOwner))
                    {

                        if (board.transferOwner(newOwner,boardDalController,userBoardDalController))
                        {
                            board.Owner = newOwner;
                            return true;
                        }
                        else
                            throw new Exception("Database failed to update board owner");
                    }
                    else
                    {
                        throw new Exception("New owner is not a member of board");
                    }
                }
                else
                {
                    throw new Exception("User is not owner of board");
                }
            }
            else
            {
                throw new Exception("User is not logged in");
            }
        }


        /// <summary>
        /// The method changes the assignee of the task, it is responsible for the functional requirement 23.
        /// </summary>
        /// <param name="email">the email of the assigner</param>
        /// <param name="boardName">the name of the board</param>
        /// <param name="columnOrdinal">the id of the column</param>
        /// <param name="taskID">the id of the task</param>
        /// <param name="emailAssignee">the email of the new assignee</param>
        /// <exception cref="Exception">inputs are invalid</exception>
        public void assignTask(string email, string boardName, int columnOrdinal, int taskID, string emailAssignee) {
            log.Debug("successfully called method assignTask in BoardManagement, attempting to assign the task " + taskID + " to a new assignee");

            um.checks(email);
            um.checks(emailAssignee);

            email = email.ToLower();
            emailAssignee = emailAssignee.ToLower();

            if (!um.isLoggedIn(email))
                throw new Exception("User is not logged in");

            User user = um.GetUser(email);
            Board board = getBoard(user, boardName);
            
            if(board.isMember(email) && board.isMember(emailAssignee))
            {

                board.assignTask(email, emailAssignee, taskID, columnOrdinal);
            }
            else
            {
                throw new Exception("One of the users not a member");
            }
        }
        

        /// <summary>
        /// this function returns the boards of the user
        /// </summary>
        /// <param name="email">the email of the user</param>
        /// <returns>a list of boards</returns>
        /// <exception cref="Exception">user isnt logged in</exception>
        public List<int> GetUserBoards(string email)
        {
            log.Debug("Successfully called method GetUserBoards in BoardManagement, attempting to get boards of user.");

            List<int> boardIDs = new List<int>();
            um.checks(email);
            email = email.ToLower();

            if (um.isLoggedIn(email))
            {
                foreach (Board board in BoardList)
                {
                    if (board.isMember(email))
                    {
                        boardIDs.Add(board.BoardId);
                    }
                }
                return boardIDs;
            }
            else
            {
                throw new Exception("User is not logged in");
            }
        }



        /// <summary>
        /// This function loads data from the database into our project's variables
        /// </summary>
        /// <returns>an empty string value</returns>
        public string LoadData() 
        {

            log.Debug("Successfully called method LoadData in BoardManagement, attempting to load data in UserManagement variables as well as BoardManagement from database");
            um.LoadData();


            List<DTO> dTOs = BoardIdCounterDalController.Select();

            foreach (DTO dto in dTOs) 
            {
                BoardIdCounterDTO idCounterDTO = (BoardIdCounterDTO)dto;

                //checking if the input is legal
                if (idCounterDTO.counter >= 0)
                {
                    BoardIdCounter = idCounterDTO.Counter;
                } 
            }

            dTOs = boardDalController.Select();
            boardList = new List<Board> ();

            foreach (DTO dto in dTOs) 
            {
               BoardDTO boardDTO = (BoardDTO)dto;
               
                //checking if boardName and owner are legal + board isnt currently in the system
                if(!boardExistsLoad(boardDTO.BoardName, boardDTO.Owner))
                {
                   boardList.Add(new Board(boardDTO.BoardName, boardDTO.Owner.ToLower(), boardDTO.BoardId, boardDTO.BackLogCapacity, boardDTO.InProgressCapacity, boardDTO.DoneCapacity));
                }  
            }

            dTOs = userBoardDalController.Select();
            foreach (DTO dto in dTOs)
            {
                UserBoardDTO userBoardDTO = (UserBoardDTO)dto;
                Board board = getBoard(userBoardDTO.BoardId);

                //checks if the user isn't already in the collaborators of this board
                if (!board.Collaborators.Contains(userBoardDTO.Email))
                {

                    board.Collaborators.Add(userBoardDTO.Email.ToLower());
                }
            }
            
            dTOs = taskCounterDalController.Select();

            foreach (DTO dto in dTOs) 
            {
                TaskIdCounterDTO taskIdCounterDTO = (TaskIdCounterDTO)dto;
                Board board = getBoard(taskIdCounterDTO.BoardId);
                
                //checks if the taskIDcounter is legal
                if(board.TaskIdCounter == taskIdCounterDTO.Counter + 1)
                {
                    board.TaskIdCounter = taskIdCounterDTO.Counter;
                }
            }

            dTOs = taskDalController.Select();

            foreach(DTO dto in dTOs)
            {
                TaskDTO taskDTO = (TaskDTO)dto;
                //check if this returns an exception, that the for loop still goes on
                Board board = getBoard(taskDTO.BoardId);
                
                if (taskDTO.ColumnIndex == 0)
                {
                    Task temp = new Task(taskDTO.Title, taskDTO.Description, taskDTO.DueDate, taskDTO.TaskId, taskDTO.BoardId, taskDTO.CreationTime, taskDTO.Assignee);
                    if (!board.BacklogList.Contains(temp))
                    {
                        board.BacklogList.Add(temp);
                    }
                }
                else if(taskDTO.ColumnIndex == 1)
                {
                    
                    Task temp = new Task(taskDTO.Title, taskDTO.Description, taskDTO.DueDate, taskDTO.TaskId, taskDTO.BoardId, taskDTO.CreationTime, taskDTO.Assignee);
                    if (!board.InProgressList.Contains(temp))
                    {
                        board.InProgressList.Add(temp);
                    }
                }
                else if (taskDTO.ColumnIndex == 2)
                {
                    Task temp = new Task(taskDTO.Title, taskDTO.Description, taskDTO.DueDate, taskDTO.TaskId, taskDTO.BoardId, taskDTO.CreationTime, taskDTO.Assignee);
                       if (!board.DoneList.Contains(temp))
                       {
                            board.DoneList.Add(temp);
                       }
                }
            }
            return "";
        }



        /// <summary>
        /// This function removes data from our project
        /// </summary>
        /// <returns>a string value</returns>
        public string RemoveData()
        {
            log.Debug("Successfully called method RemoveData in BoardManagement, attempting to clear all project variables in BoardManagement and UserManagement.");
           
            um.RemoveData();

            if (!boardDalController.Delete())
                throw new Exception("Database failed to remove Board table");

            if(!boardIdCounterDalController.Delete())
                throw new Exception("Database failed to remove boardid table");

            if (!userBoardDalController.Delete())
                throw new Exception("Database failed to remove userBoard table");

            if (!taskCounterDalController.Delete())
                throw new Exception("Database failed to remove taskCounter table");

            if (!taskDalController.Delete())
                throw new Exception("Database failed to remove task table");
            

            boardList.Clear();
            BoardIdCounter = 0;
            return "";
        }
    }
}