using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Frontend.Model
{
    public class BoardModel
    {
        //fields
        private string boardName;
        private List<TaskModel> backlog;
        private List<TaskModel> inProgress;
        private List<TaskModel> doneList;
        private string owner;


        //constructor
        [JsonConstructor]
        public BoardModel(string boardName, List<TaskModel> backlogList, List<TaskModel> inProgressList, List<TaskModel> doneList, string owner)
        {
            this.BoardName = boardName;
            this.BacklogList = backlogList;
            this.InProgressList = inProgressList;
            this.DoneList = doneList;
            this.Owner = owner;

        }

         
        //getters and setters
        public string BoardName { get { return boardName; } set { this.boardName = value; } }
        public List<TaskModel> BacklogList { get { return backlog; } set { backlog = value;} }
        public List<TaskModel> InProgressList { get { return inProgress; } set { inProgress = value; } }
        public List<TaskModel> DoneList { get { return doneList; } set { doneList = value; } }
        public string Owner { get { return owner; } set { this.owner = value; } }
    }
}
