using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntroSE.Kanban.Backend.DataAccessLayer
{
    public class BoardDTO : DTO
    {
        //fields
        public const string boardNameColumnName = "boardName";
        public string boardName;
        
        public const string ownerNameColumnName = "owner";
        public string owner;
        
        public const string idColumnName = "boardId";
        public int boardId;

        public const string backLogCapacity_columnName = "capacity_backLog";
        public int backLogCapacity;
        
        public const string inProgressCapacity_columnName = "capacity_inProgress";
        public int inProgressCapacity;
        
        public const string doneCapacity_columnName = "capacity_done";
        public int doneCapacity;
       
        //constructor
        public BoardDTO(string BoardName, string Owner,int BoardId, int BackLogCapacity, int InProgressCapacity,int DoneCapacity):base(new BoardDalController())
        {
            this.boardName = BoardName;
            owner = Owner;
            boardId = BoardId;
            backLogCapacity = BackLogCapacity;
            inProgressCapacity = InProgressCapacity;
            doneCapacity = DoneCapacity;
        }

        //getters and setters
        public int DoneCapacity { get => doneCapacity; set { doneCapacity = value; _controller.Update(boardId, doneCapacity_columnName, value, "boardId"); } }

        public int InProgressCapacity { get => inProgressCapacity; set { inProgressCapacity = value; _controller.Update(boardId, inProgressCapacity_columnName, value, "boardId"); } }
        
        public int BackLogCapacity { get => backLogCapacity; set { backLogCapacity = value; _controller.Update(boardId, backLogCapacity_columnName, value, "boardId"); } }
        
        public string Owner { get => owner; set { owner = value; _controller.Update(boardId, ownerNameColumnName, value, "boardId"); } }
        
        public string BoardName { get => boardName; set { boardName = value; _controller.Update(boardId, boardNameColumnName, value, "boardId"); } }
        
        public int BoardId { get => boardId; }

        
    }
}
