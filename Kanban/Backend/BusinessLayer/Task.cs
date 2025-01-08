using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using log4net;
using log4net.Config;
using System.IO;
using System.Reflection;
using IntroSE.Kanban.Backend.DataAccessLayer;

namespace IntroSE.Kanban.Backend.BusinessLayer
#nullable enable
{
    public class Task
    {
        /// <summary>
        /// Task is an object which is editable, it is generally saved in the columns of the Board
        /// </summary>
        /// <param name="title">title of the task</param>
        /// <param name="description">description of the task</param>
        /// <param name="creationTime">the creation time of the task</param>
        /// <param name="dueDate">the due date of the task</param>
        /// <param name="columnIndex">the index of the column</param>
        /// <param name="taskId">id of the task</param>
        /// <param name="assignee">the assigne</param>
        /// <param name="boardID">id of the board</param>


        //fields
        private string title;
        private string description;
        private DateTime creationTime;
        private DateTime dueDate;
        private int columnIndex; //  0 -> backlog , 1 -> inprogress, 2 -> done
        private int taskId;
        private string assignee;
        private int boardID;

        private readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        //magic numbers
        private readonly int MAX_COLUMN_INDEX = 2;
        private readonly int MIN_COLUMN_INDEX = 0;
        private readonly int MIN_TITLE_LENGTH = 0;
        private readonly int MAX_TITLE_LENGTH = 50;
        private readonly int MAX_DESC_LENGTH = 300;


        //constructors
        public Task(string title, string description, DateTime dueDate, int taskId, int boardId)
        {
            //logging setup
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
            log.Info("Task " + title + " was created");

            Title = title;
            Description = description;
            CreationTime = DateTime.Now;
            DueDate = dueDate;
            ColumnIndex = 0;
            TaskId = taskId;
            Assignee = null;
            BoardId = boardId;

        }

        public Task(string title, string description, DateTime dueDate, int taskId, int boardId, DateTime creationTime, string? assignee)
        {
            //logging setup
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
            log.Info("Task " + title + " was loaded!");

            Title = title;
            Description = description;
            CreationTime = DateTime.Now;
            DueDate = dueDate;
            ColumnIndex = 0;
            TaskId = taskId;
            BoardId = boardId;
            CreationTime = creationTime;
            if(assignee != null)
            {
                Assignee = assignee.ToLower();
            }
            else
            {
                Assignee = null;
            }
           
        }

        [JsonConstructor]
        public Task(int id, DateTime creationTime, string title, string description, DateTime dueDate)
        {
            TaskId = id;
            CreationTime = creationTime;
            Title = title;
            Description = description;
            DueDate = dueDate;

        }


        //getters and setters
        public int TaskId { get { return taskId; } set { taskId = value; } }

        public DateTime CreationTime { get { return creationTime; } set { dueDate = value; } }

        public string Title { get { return title; } set { title = value; } }

        public string Description { get { return description; } set { description = value; } }

        public DateTime DueDate { get { return dueDate; } set { dueDate = value; } }

        [JsonIgnore]
        public int ColumnIndex { get { return columnIndex; } set { columnIndex = value; } }
        [JsonIgnore]
        public string Assignee { get { return assignee; } set { assignee = value; } }

        [JsonIgnore]
        public int BoardId { get => boardID; set { boardID = value; } }

        
        //methods

        ///<summary>
        ///This function edits the editTitle method, according to functional requirerment 15. We make sure that task is not in "done" state- according to functional requirerment 14.
        /// </summary>
        /// <param name="columnIndex"></param>
        /// <param name="taskDalController">responsible for updating the task entry in the database</param>
        /// <returns>true if the setIndex was successful and false otherwise</returns>
        /// <exception cref="Exception">when column doesnt exists</exception>
        public bool setIndex(int columnIndex,TaskDalController taskDalController)
        {
            log.Debug("successfully called method setIndex in Task, setting the column index as " + columnIndex);

            if (columnIndex < MIN_COLUMN_INDEX | columnIndex > MAX_COLUMN_INDEX)
            {
                throw new Exception("Column does not exist");
            }
            if (taskDalController.Update(TaskId, BoardId, "columnIndex", columnIndex))
            {
                ColumnIndex = columnIndex;
                return true;
            }
            else
            {
                throw new Exception("Database failed to update columnIndex of task");
            }

            throw new Exception("Database faild to update task column");

        }


        /// <summary>
        ///This function edits the editTitle method, according to functional requirerment 15. 
        ///We make sure that task is not in "done" state- according to functional requirerment 14.
        /// </summary>
        /// <param name="newTitle"></param>
        /// <param name="taskDalController">responsible for the updating of the task entry in the database</param>
        /// <exception cref="Exception">title is illegal or task is in done list and cannot be changed</exception>
        public void editTitle(string newTitle, TaskDalController taskDalController)
        {

            log.Debug("successfully called method editTitle in Task, editing the title to " + newTitle);

            if (ColumnIndex != MAX_COLUMN_INDEX)
            {
                if (newTitle != null && newTitle.Length > MIN_TITLE_LENGTH & newTitle.Length <= MAX_TITLE_LENGTH)
                {
                    if (taskDalController.Update(taskId, BoardId, "title", newTitle))
                        Title = newTitle;
                    else
                        throw new Exception("Database failed to edit task title");
                }
                else
                {
                    throw new Exception("new title does not meet the requirements, is illegal");
                }

            }
            else if (ColumnIndex == MAX_COLUMN_INDEX)
            {
                throw new Exception("task is in done list and can not be changed");
            }
        }


        /// <summary>
        /// 
        ///This function edits the editDescription method, according to functional requirerment 15. 
        ///We make sure that task is not in "done" state- according to functional requirerment 14.
        /// </summary>
        /// <param name="newDesc"></param>
        /// <param name="taskDalController">responsible for updating the task entry in the database</param>
        /// <exception cref="Exception">description is illegal or task is in done list and cannot be changed</exception>
        public void editDescription(string newDesc, TaskDalController taskDalController)
        {
            log.Debug("successfully called the method editDescription in Task, editing the task description to" + newDesc);

            if (ColumnIndex != MAX_COLUMN_INDEX)
            {
                if (newDesc != null && newDesc.Length <= MAX_DESC_LENGTH)
                {
                    if (taskDalController.Update(taskId, BoardId, "desc", newDesc))
                        Description = newDesc;
                    else
                        throw new Exception("Database failed to edit task description");
                }
                else
                {
                    throw new Exception("new descrpition does not meet the requirements, is illegal");
                }
            }

            else if (ColumnIndex == MAX_COLUMN_INDEX)
            {
                throw new Exception("task is in done list and can not be changed");
            }

        }


        /// <summary>
        ///This function edits the editDueDate method, according to functional requirerment 15.
        ///We make sure that task is not in "done" state- according to functional requirerment 14.
        /// </summary>
        /// <param name="newDueDate"></param>
        /// <param name="taskDalController">responsible for updating the task entry in the database</param>
        /// <exception cref="Exception">duedate is illegal or task is in done list and cannot be changed</exception>
        public void editDueDate(DateTime newDueDate, TaskDalController taskDalController)
        {
            log.Debug("successfully called the method editDueDate in Task, editing the task dueDate" + newDueDate);

            if (ColumnIndex != MAX_COLUMN_INDEX)
            {
                if (DateTime.Compare(newDueDate, DateTime.Now) > 0)
                {
                    if (taskDalController.Update(taskId, BoardId, "dueDate", newDueDate))
                        DueDate = newDueDate;
                    else
                        throw new Exception("Database failed to edit task dueDate");
                }
                else
                {
                    throw new Exception("new dueDate does not meet the requirements, is illegal");
                }
            }

            else if (ColumnIndex == MAX_COLUMN_INDEX)
            {
                throw new Exception("task is in done list and can not be changed");
            }
        }
    }
}