using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntroSE.Kanban.Backend.DataAccessLayer
{
    public class TaskDTO:DTO
    {
        //fields
        public const string taskIdColumnName = "taskId";
        public const string titleColumnName = "title";
        public const string descriptionColumnName = "desc";
        public const string creationTimeColumnName = "creationTime";
        public const string dueDateColumnName = "dueDate";
        public const string boardIdColumnName = "boardId";
        public const string columnIndexColumnName = "columnIndex";
        public const string assigneeColumnName = "assignee";

        public int taskId;
        public string title;
        public string desc;
        public DateTime creationTime;
        public DateTime dueDate;
        public int boardId;
        public int columnIndex;
        public string assignee;


        //constructor
        public TaskDTO(int TaskId,string Title,string Desc,DateTime CreationTime, DateTime DueDate, int BoardId,int ColumnIndex,string Assignee):base(new TaskDalController())
        {
            taskId= TaskId;
            title= Title;
            desc= Desc;
            creationTime= CreationTime;
            dueDate= DueDate;
            boardId= BoardId;
            columnIndex = ColumnIndex;
            assignee= Assignee;

        }


        //getters and setters
        public int TaskId { get => taskId; }

        public string Title { get => title; set { title = value; _controller.Update(taskId, boardId, "title", value); } }
        
        public string Description { get => desc; set { desc = value; _controller.Update(taskId, boardId, "desc", value); } }
        
        public DateTime CreationTime { get => creationTime; set { creationTime = value; _controller.Update(taskId, boardId, "creationTime", value); } }
        
        public DateTime DueDate { get => dueDate; set { dueDate = value; _controller.Update(taskId, boardId, "dueDate", value); } }
        
        public int BoardId { get => boardId; }
        
        public int ColumnIndex { get => columnIndex; set { columnIndex = value; _controller.Update(TaskId, boardId, "columnIndex", value); } }
        
        public string Assignee { get => assignee; set { assignee = value; _controller.Update(TaskId, BoardId, "assignee", value); } }

    }
}