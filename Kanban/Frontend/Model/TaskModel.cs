using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Frontend.Model
{
    public class TaskModel
    {
        //fields
        private int taskId;
        private DateTime creationTime;
        private string title;
        private string description;
        private DateTime dueDate;


        //constructor
        /// <summary>
        /// Json Constructor of the TaskModel
        /// </summary>
        /// <param name="taskId">the id of the current task</param>
        /// <param name="CreationTime">the creation time of the current task</param>
        /// <param name="title">the title of the current task</param>
        /// <param name="description">the description of the current task</param>
        /// <param name="dueDate">the duedate of the current task</param>
        [JsonConstructor]
        public TaskModel(int taskId, DateTime creationTime, string title, string description, DateTime dueDate)
        {
            TaskId = taskId;
            CreationTime = creationTime;
            Title = title;
            Description = description;
            DueDate = dueDate;
            CreationTime = creationTime;

        }


        //getters and setters
        public int TaskId { get { return taskId; } set { this.taskId = value;} }
        public DateTime CreationTime { get { return creationTime; } set { this.creationTime = value;} }
        public string Title { get { return title; } set { this.title = value; } }
        public string Description { get { return description; } set { this.description = value; } }
        public DateTime DueDate { get { return dueDate; } set { this.dueDate = value; } }


        public string StringTaskId { get { return "ID : " + taskId; } }// set { this.taskId = value;} }
        public string StringCreationTime { get { return "Creation Time : " + creationTime; } } // set { this.creationTime = value;} }
        public string StringTitle { get { return "Title : " + title; } } //set { this.title = value; } }
        public string StringDescription { get { return "Description : " + description; } }// set { this.description = value; } }
        public string StringDueDate { get { return "Due date : " + dueDate; } } //set { this.dueDate = value; } }
    }
}
