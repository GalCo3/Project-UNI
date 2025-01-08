using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntroSE.Kanban.Backend.DataAccessLayer
{
    public class UserDTO:DTO
    {
        //fields
        public const string emailColumnName = "email";
        public const string passwordColumnName = "password";

        public string email;
        public string password;


        //constructor
        public UserDTO(string Email,string Password) : base(new UserDalController()) 
        { 
            email = Email;
            password = Password;
        }


        //getters and setters
        public string Email { get => email; }

        public string Password { get => password; }
        
    }
}