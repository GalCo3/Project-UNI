using System;
using System.Collections.Generic;
using System.Windows;
using IntroSE.Kanban.Backend.ServiceLayer;
//using Backend.Service.Objects;

namespace Frontend.Model
{
    public class BackendController
    {
        //fields
        private UserService UserService { get; set; }
        private BoardService BoardService { get; set; }


        //constructor

        /// <summary>
        /// The following is a constructor of BackendController that rececieves UserService and BoardService as fields
        /// </summary>
        /// <param name="userService"> resonsible for communication with the user's backend methods</param>
        /// <param name="boardService"> responsible for communication with the board's backend methods</param>
        public BackendController(UserService userService, BoardService boardService)
        {
            UserService = userService;
            BoardService = boardService;
        }


        //empty constructor
        public BackendController()
        {
            UserService = new UserService();
            BoardService = new BoardService(UserService);
            BoardService.LoadData();
        }


        //methods

        /// <summary>
        /// Method responsible for logging in user
        /// </summary>
        /// <param name="username">email of the user</param>
        /// <param name="password">password of the user</param>
        /// <returns>A userModel based on the user we logged in</returns>
        /// <exception cref="Exception">Throws exception when login failed</exception>
        public UserModel Login(string username, string password)
        {
            string json = UserService.login(username, password);
            Response<string> user = JsonController.DeSerialize<Response<string>>(json);
            if (user.ErrorMessage!= null)
            {
                throw new Exception(user.ErrorMessage);
            }
            return new UserModel(this, username);
        }


        /// <summary>
        /// Method responsible for logging out the user from the program
        /// </summary>
        /// <param name="username">email of the user</param>
        /// <returns>null if there are no error messages </returns>
        /// <exception cref="Exception">Throws exception if there was a problem in logging out the user</exception>
        public string Logout(string username)
        {
            string json = UserService.logout(username);
            Response<string> user = JsonController.DeSerialize<Response<string>>(json);
            if (user.ErrorMessage != null)
            {
                throw new Exception(user.ErrorMessage);
            }
            return user.ErrorMessage;
        }

        /// <summary>
        /// Method responsible for listing the user's boards 
        /// </summary>
        /// <param name="username">username of the user that is logged in</param>
        /// <returns>A list of boardModels that represent the boards for frontend display</returns>
        /// <exception cref="Exception">Throws exception when listing boards of user failed</exception>
        public UserBoardModel ListUserBoards(string username)
        {
            string json = BoardService.getUsersBoards(username);
            Response<List<BoardModel>> user = JsonController.DeSerialize<Response<List<BoardModel>>>(json);
            if (user.ErrorMessage != null)
            {
                throw new Exception(user.ErrorMessage);
            }
            List<BoardModel> boards = user.ReturnValue;
            return new UserBoardModel(this,username,boards);

        }
       

        /// <summary>
        /// Method responsible for registering the user into program
        /// </summary>
        /// <param name="username">email of the new user</param>
        /// <param name="password">password of the new user</param>
        /// <exception cref="Exception">Throws exception when creating the new user failed to register</exception>
        internal void Register(string username, string password)
        {
            string json = UserService.createNewUser(username, password);
            Response<string> res = JsonController.DeSerialize<Response<string>>(json);
            if (res.ErrorMessage!= null)
            {
                throw new Exception(res.ErrorMessage);
            }
        }
    }
}
