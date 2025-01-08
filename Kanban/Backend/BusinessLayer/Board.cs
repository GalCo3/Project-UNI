using IntroSE.Kanban.Backend.DataAccessLayer;
using log4net;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.Json.Serialization;


namespace IntroSE.Kanban.Backend.BusinessLayer
#nullable enable
{

    public class Board
    {
        /// <summary>
        /// The Following class defines the object board in our assignment, each board is defined by the name of the board,
        /// the email of the user it created, the three lists that are requested and their capacities in an array.
        /// </summary>
        ///<param name="boardName"> the name of the board</param>
        ///<param name="backLogList"> list of all tasks in the backlog column</param>
        ///<param name="inProgressList"> list of all tasks in the inprogress column</param>
        ///<param name="doneList">list of all tasks in the done column</param>
        ///<param name="email"> email of the user which created the board</param>
        ///<param name="capacities"> array of capacities of each column</param>
        ///<param name="taskIdCounter"> counter to assign an id for each task</param>
        ///<param name="boardId">an id for a board</param>
        ///<param name="collaborators">a list of collaborators</param>



        //fields
        public string boardName;
        private List<Task> backlogList;
        private List<Task> inProgressList;
        private List<Task> doneList;
        private string owner;
        private int[] capacities; //array of capacities of each list
        private int taskIdCounter;
        private int boardId;
        private List<string> collaborators;

        //Log
        private readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);


        //magic numbers
        private readonly int MAX_COLUMN_INDEX = 2;
        private readonly int MIN_COLUMN_INDEX = 0;
        private readonly int MIN_TITLE_LENGTH = 0;
        private readonly int MAX_TITLE_LENGTH = 50;
        private readonly int MAX_DESC_LENGTH = 300;

        //constructors
        public Board(string boardName, string email, int boardId)
        {
            //logging setup
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
            log.Info("New board created:" + boardName);


            BoardName = boardName;
            BoardId = boardId;

            BacklogList = new List<Task>();
            InProgressList = new List<Task>();
            DoneList = new List<Task>();
            Capacities = new int[] { -1, -1, -1 }; //stores the capacities of the three lists, -1 means hasn't been limited yet

            Collaborators = new List<string>();
            Owner = email.ToLower(); //email of user who created the board
            Collaborators.Add(email.ToLower());

            TaskIdCounter = 0;
        }

        public Board(string boardName, string email, int boardId, int backLogCapacity, int inProgressCapacity, int DoneCapacity)
        {
            //logging setup
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
            log.Info("Board: " + boardName + " is loaded!");


            BoardName = boardName;
            BoardId = boardId;

            BacklogList = new List<Task>();
            InProgressList = new List<Task>();
            DoneList = new List<Task>(); 
            Capacities = new int[] { backLogCapacity, inProgressCapacity, DoneCapacity }; //stores the capacities of the three lists, -1 means hasn't been limited yet

            Collaborators = new List<string>();
            Owner = email.ToLower(); //email of user who created the board
            Collaborators.Add(email.ToLower());

            TaskIdCounter = 0;
        }

        [JsonConstructor]
        public Board(string boardName, List<Task> backlogList, List<Task> inProgressList, List<Task> doneList, string email)
        {
            BoardName = boardName;
            BacklogList = backlogList;
            InProgressList = inProgressList;
            DoneList = doneList;
            Owner = email;
            
        }

        //getters and setters
        public string BoardName { get { return boardName; } set { boardName = value; } }

        public List<Task> BacklogList { get { return backlogList; } set { backlogList = value; } }

        public List<Task> InProgressList { get { return inProgressList; } set { inProgressList = value; } }

        public List<Task> DoneList { get { return doneList; } set { doneList = value; } }

        public string Owner { get { return owner; } set { owner = value; } }

        [JsonIgnore]
        public int[] Capacities { get { return capacities; } set { capacities = value; } }

        [JsonIgnore]
        public int TaskIdCounter { get { return taskIdCounter; } set { taskIdCounter = value; } }

        [JsonIgnore]
        public int BoardId { get { return boardId; } set { boardId = value; } }

        [JsonIgnore]
        public List<string> Collaborators { get { return collaborators; } set { collaborators = value; } }
        

        //methods


        /// <summary>
        ///This method returns the capacity of BackLogList/InProgressList/DoneList based on the index chosen, used to compliment methods for the
        ///functional requirement 10.
        ///0 -> BackLogList
        ///1 -> InProgressList
        ///2 -> DoneList
        /// </summary>
        /// <param name="columnIndex">index of the column</param>
        /// <returns> return the capacity of the chosen column, -1 being no capacity limit</returns>
        /// <exception cref="Exception">when index is out of bounds</exception>
        public int getCapacity(int columnIndex)
        {
            log.Debug("successfully called method getCapacity in Board, Getting capacity of column " + columnIndex + " in board " + BoardName);

            if (columnIndex <= MAX_COLUMN_INDEX & columnIndex >= MIN_COLUMN_INDEX)
            {
                return capacities[columnIndex];
            }
            else
            {
                throw new Exception("index out of bounds, can only be between 0 to 2");
            }
        }


        /// <summary>
        /// getting access to all tasks in a chosen column
        /// </summary>
        /// <param name="columnOrdinal">index of the column</param>
        /// <returns> returns a List<Task></returns>
        /// <exception cref="Exception">when index is out of bounds, therefore no such column to return</exception>
        public List<Task> getColumn(int columnOrdinal)
        {
            log.Debug("successfully called method getColumn in Board, Getting column " + columnOrdinal + " in board " + BoardName);

            if (columnOrdinal == 0)
                return backlogList;

            if (columnOrdinal == 1)
                return inProgressList;

            if (columnOrdinal == 2)
                return doneList;


            throw new Exception("Column does not exist");
        }


        /// <summary>
        /// This method is responsible for the functional requirement 19, listing all progress tasks that are assigned to a user
        /// </summary>
        /// <param name="email">email of the user</param>
        /// <returns>returns a list with tasks</returns>
        public List<Task> listInProgressTask(string email)
        {
            log.Debug("Successfully called ListInProgressTask in Board, listing all inProgress tasks assigned to user in board " + BoardName);
            List<Task> list = new List<Task>();

            foreach (Task task in inProgressList)
            {
                if (task.Assignee.Equals(email))

                    list.Add(task);
            }

            return list;
        }


        /// <summary>
        ///This function returns the corresponding task according to the task id that is given. 
        /// </summary>
        /// <param name="taskId">id of task</param>
        /// <returns>returns a task</returns>
        /// <exception cref="Exception">when task doesnt exist</exception>
        public Task getTask(int taskId)
        {
            log.Debug("succesfully called method getTask in Board, Getting task " + taskId + " in board " + BoardName);

            foreach (Task task in backlogList)
            {
                if (task.TaskId == taskId)
                {
                    return task;
                }
            }

            foreach (Task task in inProgressList)
            {
                if (task.TaskId == taskId)
                {
                    return task;
                }
            }

            foreach (Task task in doneList)
            {
                if (task.TaskId == taskId)
                {
                    return task;
                }
            }

            throw new Exception("Task doesnt exist.");
        }


        /// <summary>
        ///This function returns the corresponding task according to the task id that is given. 
        /// <param name="columnOrdinal">index of the column</param>
        /// <param name="taskId">a task id</param>
        /// <returns>A task</returns>
        /// <exception cref="Exception">when task doesnt exist or when column ordinal not valid</exception>
        public Task getTask(int columnOrdinal, int taskId)
        {
            log.Debug("successfully called method getTask using columnOrdinal in Board, getting task " + taskId + " in board " + BoardName);

            if (columnOrdinal == 0)
            {
                foreach (Task task in backlogList)
                {
                    if (task.TaskId == taskId)
                        return task;
                }
                log.Warn("passed first validity check but failed inner check, missing task or columnOrdinal pointing to wrong column");

                throw new Exception("columnOrdinal is 0 but task wasn't found in backlogList");
            }

            if (columnOrdinal == 1)
            {

                foreach (Task task in inProgressList)
                {
                    if (task.TaskId == taskId)
                        return task;
                }
                log.Warn("passed second validity check but failed inner check, missing task or columnOrdinal pointing to wrong column");

                throw new Exception("columnOrdinal is 1 but task wasn't found in inProgressList");
            }

            if (columnOrdinal == 2)
            {
                foreach (Task task in doneList)
                {
                    if (task.TaskId == taskId)
                        return task;
                }
                log.Warn("passed third validity check but failed inner check, missing task or columnOrdinal pointing to wrong column");

                throw new Exception("columnOrdinal is 2 but task wasn't found in doneList");
            }

            throw new Exception("Task doesnt exist. Or column ordinal not valid");
        }


        /// <summary>
        ///This function sets the capacity for one of the three lists by a given index, according to functional requirerment 10.
        ///0 -> backLogList
        ///1 -> inProgressList
        ///2 -> doneList
        /// </summary>
        /// <param name="limit">a limit for the tasks</param>
        /// <param name="columnIndex">index of the column</param>
        /// <param name="boardDalController">responsible for updating the capacity in the database</param>
        /// <exception cref="Exception">capacity error</exception>
        public void setCapacity(int limit, int columnIndex,BoardDalController boardDalController)
        {
            log.Debug("successfully called method setCapacity in Board, setting capacity of column " + columnIndex + " with new limit in board " + BoardName);

            if (limit >= -1 & columnIndex <= MAX_COLUMN_INDEX & columnIndex >= MIN_COLUMN_INDEX)
            {
                if (columnIndex == 0)
                {
                    if (backlogList.Count <= limit | limit == -1)
                    {
                        if (boardDalController.Update(boardId, "capacity_backLog", limit, "boardId"))
                            capacities[columnIndex] = limit;
                        else
                            throw new Exception("Database failed to update capacity");
                    }
                    else
                    {
                        throw new Exception("more elements in backLogList than the capacity proposed");
                    }

                }
                else if (columnIndex == 1)
                {
                    if (inProgressList.Count <= limit | limit == -1)
                    {
                        if(boardDalController.Update(boardId, "capacity_inProgress", limit, "boardId"))
                            capacities[columnIndex] = limit;
                        else
                            throw new Exception("Database failed to update capacity");
                    }
                    else
                    {
                        throw new Exception("more elements in inProgressList than the capacity proposed");
                    }

                }
                else if (columnIndex == 2)
                {
                    if (doneList.Count <= limit | limit == -1)
                    {
                        if (boardDalController.Update(boardId, "capacity_done", limit, "boardId"))
                            capacities[columnIndex] = limit;
                        else
                            throw new Exception("more elements in inProgressList than the capacity proposed");
                    }
                    else
                    {
                        throw new Exception("more elements in DoneList than the capacity proposed");
                    }
                }
            }
            else
            {
                throw new Exception("limit must be greater/equal than/to 0 and column index must be one of the three: 0,1,2");
            }
        }


        /// <summary>
        ///This function adds a task to the backloglist, according to functional requirerment 12, by first ensuring the capacity and then adding 
        ///if the capacity allows for such
        /// </summary>
        /// <param name="title">task title</param>
        /// <param name="desc">task description</param>
        /// <param name="dueDate">task duedate</param>
        /// <param name="taskDalController">responsible for adding the task into the database</param>
        /// <returns>returns a task</returns>
        /// <exception cref="Exception">input is illegal</exception>
        public Task addTask(string title, string desc, DateTime dueDate,TaskDalController taskDalController)
        {
            log.Debug("successfully called addTask method in Board, adding task " + title + " to board " + BoardName);

            //in order to addTask, we must verify the capacity isn't reached in backLog 

            if (capacities[0] == -1 || backlogList.Count < capacities[0])
            {
                if (title == null | desc == null)
                    throw new Exception("Title / description can not be null");

                if (DateTime.Compare(dueDate, DateTime.Now) <= 0)
                    throw new Exception("duedate is illegal");

                if (title.Length > MIN_TITLE_LENGTH & title.Length <= MAX_TITLE_LENGTH & desc.Length <= MAX_DESC_LENGTH)
                {
                    if (taskDalController.Insert(new TaskDTO(TaskIdCounter, title, desc, DateTime.Now, dueDate, BoardId, 0, null)))
                    {
                        Task newTask = new Task(title, desc, dueDate, TaskIdCounter, BoardId);

                        TaskIdCounter++;

                        backlogList.Add(newTask);

                        return newTask;
                    }
                    else
                    {
                        throw new Exception("Database failed to insert new Task");
                    }
                }
                else
                    throw new Exception("Title / description is illegal");
            }
            else
            {
                throw new Exception("backlog list has reached full capacity");
            }
        }


        /// <summary>
        ///This function moves a task, according to functional requirerment 13, this is done in a few steps:
        ///a. find the location of task, in one of the lists or not even found at all
        ///b. if in backLogList, ensure that inProgressList has a sufficient capacity size for adding
        ///c. if in progressList, ensure that the doneList has a sufficient capacity size for adding
        ///d. if in doneList or not found, can't be moved to anywhere
        /// </summary>
        /// <param name="columnOrdinal">index of the column</param>
        /// <param name="taskId">a task id</param>
        /// <param name="email">an email</param>
        /// <param name="taskDalController">responsible for updating the task's columnIndex in the database</param>
        /// <exception cref="Exception">capacity error</exception>
        public void moveTask(int columnOrdinal, int taskId, string email,TaskDalController taskDalController)
        {
            log.Debug("succesfully called method moveTask in Board, attempting to move task " + taskId + " in board " + BoardName);

            Task task = getTask(taskId);

            if (task.Assignee == null)
                throw new Exception("Assignee is null, no one can move the task");

            if (!task.Assignee.Equals(email))
            {
                throw new Exception("User is not assigned to this task");
            }

            //the task is found in backlogList
            if (columnOrdinal == 0 & backlogList.Contains(task))
            {

                //verifying the capacity of inProgressList allows inserting for another task
                if (capacities[1] == -1 || inProgressList.Count < capacities[1])
                {
                    if (task.setIndex(1, taskDalController))
                    {
                        backlogList.Remove(task);
                        inProgressList.Add(task);
                    }
                }
                else
                {
                    throw new Exception("inProgressList has reached full capacity");
                }

                //the task is found in inProgressList
            }
            else if (columnOrdinal == 1 & inProgressList.Contains(task))
            {

                //verifying the capacity of DoneList allows inserting for another task
                if (capacities[2] == -1 || doneList.Count < capacities[2])
                {
                    if (task.setIndex(2, taskDalController))
                    {
                        inProgressList.Remove(task);
                        doneList.Add(task);
                    }
                }
                else
                {
                    throw new Exception("doneList has reached full capacity");
                }

                //task is found in done list
            }
            else if (columnOrdinal == 2 & DoneList.Contains(task))
            {
                throw new Exception("task found in doneList or has the columnIndex of doneList and therefore can't be moved");
            }
            else
            {
                throw new Exception("task not found at all");
            }
        }


        /// <summary>
        ///This function edits a task, according to functional requirerment 15. 
        /// </summary>
        /// <param name="columnOrdinal">index of the column</param>
        /// <param name="taskId">a task id</param>
        /// <param name="newTitle">a title</param>
        /// <param name="newDesc">a description</param>
        /// <param name="newDueDate">due date</param>
        /// <param name="numAct">number of the action</param>
        /// <param name="email">an email</param>
        /// <param name="taskDalController">responsible for updating the task's parameters in the database entry</param>
        /// <exception cref="Exception">input is null</exception>
        public void editTask(int columnOrdinal, int taskId, string? newTitle, string? newDesc, DateTime? newDueDate, int numAct, string email,TaskDalController taskDalController)
        {
            log.Debug("successfully called method editTask in Board, attempting to edit task " + taskId + " in board " + BoardName);

            Task task = getTask(columnOrdinal, taskId);

            if (task.Assignee == null)
                throw new Exception("Assignee is null, no one can edit the task");

            if (!task.Assignee.Equals(email))
                throw new Exception("User is not assigned to this task");

            //checking if title should be changed
            if (newTitle != null)
            {
                task.editTitle(newTitle,taskDalController);

            }
            else if (newTitle == null & numAct == 0)
            {
                throw new Exception("New title can not be null");
            }

            //checking if description should be changed
            if (newDesc != null)
            {
                task.editDescription(newDesc,taskDalController);
            }
            else if (newDesc == null & numAct == 1)
            {
                throw new Exception("New description can not be null");
            }

            //checking if dueDate should be changed
            if (newDueDate != null)
            {
                task.editDueDate((DateTime)newDueDate, taskDalController);
            }
            else if (newDueDate == null & numAct == 2)
                throw new Exception("New DueDate can not be null");
        }


        /// <summary>
        /// This method validates that a user is indeed a member of this board
        /// </summary>
        /// <param name="email">email of the user</param>
        /// <returns>true or throws exception based on whether or not user is a member</returns>
        public bool isMember(string email)
        {
            log.Debug("Successfully called method isMember in Board, checking if user is member of board " + BoardName);

            if (email == null)
            {
                throw new Exception("Email is null");
            }

            if (collaborators.Contains(email))
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        /// Adds email of a user to the list of members of this board
        /// </summary>
        /// <param name="email">Email of user</param>
        public void addMember(string email)
        {
            log.Debug("Successfully called method addMember in Board, adding user as member to board " + BoardName);

            collaborators.Add(email);
        }


        /// <summary>
        /// Removes email of a user from the list of members of this board
        /// </summary>
        /// <param name="email">Email of user</param>
        /// <param name="taskDalController">responsible for unassigning the member from their tasks in the database</param>
        /// <returns>true if the removal of the member was successful and false otherwise</returns>
        public bool removeMember(string email,TaskDalController taskDalController)
        {
            log.Debug("Successfully called method removeMember in Board, attempting to remove user from board " + BoardName);

            //unassigns all assigned tasks in backlogList
            List < DTO > list = taskDalController.Select(email, boardId);

            if (taskDalController.Update(email, "assignee", null, "assignee"))
            {

                foreach (Task task in BacklogList)
                {
                    if (task.Assignee.Equals(email))
                    {
                        task.Assignee = null;
                    }
                }

                //unassigns all assigned tasks in InprogressList
                foreach (Task task in InProgressList)
                {
                    if (task.Assignee.Equals(email))
                    {
                        task.Assignee = null;
                    }
                }

                collaborators.Remove(email);
                return true;
            }
            return false;
        }


        /// <summary>
        /// The method changes the assignee of the task, it is responsible for the functional requirement 23.
        /// </summary>
        /// <param name="assignee">the old assignee of the task</param>
        /// <param name="newAssignee">the new assignee of the task</param>
        /// <param name="taskId">the id of the task</param>
        /// <param name="columnOrdinal">the id of the column</param>
        /// <exception cref="Exception">task is in done list or email is invalid</exception>
        public void assignTask(string assignee, string newAssignee, int taskId, int columnOrdinal)
        {
            log.Debug("Successfully called method assignTask in Board, attempting to assign task " + taskId + " to a new user in board " + BoardName);

            if (columnOrdinal == MAX_COLUMN_INDEX)
            {
                throw new Exception("Cannot reassign task in done list");
            }
            if (columnOrdinal < MIN_COLUMN_INDEX || columnOrdinal > MAX_COLUMN_INDEX)
                throw new Exception("Column ordinal not good");

            Task task = getTask(columnOrdinal, taskId);
            if (task.Assignee == null || task.Assignee.Equals(assignee))
            {
                task.Assignee = newAssignee;
                TaskDTO taskDTO = new TaskDTO(task.TaskId, task.Title, task.Description, task.CreationTime, task.DueDate, BoardId, task.ColumnIndex, task.Assignee);
                taskDTO.Assignee = newAssignee;
            }
            else
            {
                throw new Exception("Email of user does not match current assignee, therefore unable to reassign task");
            }
        }


        /// <summary>
        /// The method transfers the ownerships of the board, to a new owner, it is responsible for the functional requirement 13.
        /// </summary>
        /// <param name="newOwner">the new owner of the board</param>
        /// <param name="boardDalController">responsible for updating the board owner in the board table</param>
        /// <param name="userBoardDalController">responsible for updating the UserBoard table in the database</param>
        /// <returns>true if the transfer was successful and false otherwise</returns>
        public bool transferOwner(string newOwner,BoardDalController boardDalController, UserBoardDalController userBoardDalController)
        {
            log.Debug("Successfully called method transferOwner in Board, attempting to transfer ownership of board " + BoardName + " to a new owner in the database.");

            if (boardDalController.Update(BoardId, "owner", newOwner, "boardId"))
            {

                if (userBoardDalController.Update(Owner, BoardId, "isOwner", "no"))
                {

                    if (userBoardDalController.Update(newOwner, BoardId, "isOwner", "yes"))
                        { return true; }
                    else
                    {
                        userBoardDalController.Update(Owner, BoardId, "isOwner", "yes");
                        boardDalController.Update(BoardId, "owner", owner,"boardId");
                    }

                }
                else 
                {
                    boardDalController.Update(BoardId, "owner", owner, "boardId");
                }
            }
            
            throw new Exception("Database failed to change board owner");

        }
    }
}
