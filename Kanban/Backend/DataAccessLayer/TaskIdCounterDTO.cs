using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntroSE.Kanban.Backend.DataAccessLayer
{
    public class TaskIdCounterDTO:DTO
    {
        //fields
        public const string counterColumnName = "counter";
        public int counter;
        
        public const string BoardIDColumnName = "boardId";
        public int boardId;
        
        
        //constructor
        public TaskIdCounterDTO(int BoardId,int Counter) : base(new TaskCounterDalController())
        {
            boardId = BoardId;
            counter = Counter;
        }


        //getters and setters
        public int Counter { get => counter; set { counter = value; _controller.Update(boardId, counterColumnName, value, BoardIDColumnName); } }

        public int BoardId { get => boardId; }
    }
}
