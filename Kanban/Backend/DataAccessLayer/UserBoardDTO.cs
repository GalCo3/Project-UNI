using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntroSE.Kanban.Backend.DataAccessLayer
{
    public class UserBoardDTO:DTO
    {
        //fields
        public const string emailColumnName = "email";
        public const string boardIdColumnName = "boardId";
        public const string isOwnerColumnName = "isOwner";

        public string email;
        public int boardId;
        public string isOwner;


        //constructor
        public UserBoardDTO(string Email, int BoardId, string IsOwner) : base(new UserBoardDalController()) { 
            email = Email;
            boardId = BoardId;
            isOwner = IsOwner;
        }


        //getters and setters
        public string Email { get => email; }

        public int BoardId { get => boardId; }
        
        public string IsOwner { get => isOwner; set { isOwner = value; _controller.Update(email, boardId, "isOwner", value); } }
    }
}
