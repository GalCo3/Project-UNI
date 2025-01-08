using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frontend.Model
{

    public class UserBoardModel : NotifiableModelObject
    {
        //fields
        private List<BoardModel> boards;
        private string username;


        //constructor

        /// <summary>
        /// The following is the constructor of the UserBoardModel object, responsible for displaying the boards of the user
        /// </summary>
        /// <param name="controller">backend controller responsible for communicating with the backend</param>
        /// <param name="username">email of the user</param>
        /// <param name="boards">list of boardModels</param>
        public UserBoardModel(BackendController controller, string username, List<BoardModel> boards) : base(controller)
        {
            this.boards = boards; 
            this.username = username;
        }


        //getters and setters
        public List<BoardModel> Boards { get { return boards; } set { this.boards = value; } }

        public string Username { get { return username; } set { this.username = value; } }

    }
}
