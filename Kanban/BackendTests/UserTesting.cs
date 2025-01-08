using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntroSE.Kanban.Backend.ServiceLayer;

namespace BackendTests
{
    class UserTesting
    {
        private UserService user;
        private int testCounter;

        public UserTesting(UserService user) {
            this.user = user;
            this.testCounter = 0;
        }

        public void runTests()
        {

            Console.WriteLine("----------------------------Testing User-------------------------------------------");

            Console.WriteLine("----------------------------Testing UserRegistration-------------------------------------------");
            registrationTests();

            Console.WriteLine("----------------------------Testing LoginTests-------------------------------------------");
            loginTest();

            Console.WriteLine("----------------------------Testing LogoutTests-------------------------------------------");
            logoutTest();
        }

        ///<summary>
        ///This function tests Functional Requirement 7 of Registering a user into the system.
        /// </summary>
        public string registrationTests(string email, string password)
        {
            return user.createNewUser(email, password);
        }





        public void registrationTests()
        {
            string res;

            Console.WriteLine("Expected to fail");

            res = registrationTests("blabla", "pass");
            Console.WriteLine("Test " + testCounter++ +" " + res);

            //all expected to fail so no need to worry about unique emails here



            Console.WriteLine("Expected to fail");

            res = registrationTests("lina@gmail.com", "Lina2");
            Console.WriteLine("Test " + testCounter++ + " " + res);



            Console.WriteLine("Expected to fail");

            res = registrationTests("lina@gmail.com", "");
            Console.WriteLine("Test " + testCounter++ + " " + res);

            
            Console.WriteLine("Expected to fail");

            res = registrationTests("lina@gmail.com", null);
            Console.WriteLine("Test " + testCounter++ + " " + res);

            
            Console.WriteLine("Expected to fail");

            res = registrationTests("", "Dolphin5");
            Console.WriteLine("Test " + testCounter++ + " " + res);

            
            Console.WriteLine("Expected to fail");

            res = registrationTests(null, "Dolphin5");
            Console.WriteLine("Test " + testCounter++ + " " + res);

            
            Console.WriteLine("Expected to fail");

            res = registrationTests("lina@gmail.com", "DolphinRainbowUnicorn123");
            Console.WriteLine("Test " + testCounter++ + " " + res);

            
            Console.WriteLine("Expected to fail");

            res = registrationTests("lina@gmail.com", "DOLPHIN");
            Console.WriteLine("Test " + testCounter++ + " " + res);

            
            Console.WriteLine("Expected to fail");

            res = registrationTests("lina@gmail.com", "dolphin");
            Console.WriteLine("Test " + testCounter++ + " " + res);


            Console.WriteLine("Expected to fail");

            res = registrationTests("lina@gmail.com", "123456");
            Console.WriteLine("Test " + testCounter++ + " " + res);

            Console.WriteLine("Expected to fail");

            res = registrationTests("lina@gmail.com", "DOLPHIN5");
            Console.WriteLine("Test " + testCounter++ + " " + res);


            Console.WriteLine("Expected to fail");

            res = registrationTests("lina@gmail.com", "dolphin5");
            Console.WriteLine("Test " + testCounter++ + " " + res);


            Console.WriteLine("Expected to fail");

            res = registrationTests("linaMasarwa", "Dolphin5");
            Console.WriteLine("Test " + testCounter++ + " " + res);


            //validating that all users have unique emails
            registrationTests("gala0@post.bgu.ac.il", "t2JKdh15");


            Console.WriteLine("Expected to fail");

            res = registrationTests("lina @gmail.com", "Dolphin5"); //supposed to fail, have space
            Console.WriteLine("Test " + testCounter++ + " " + res);


            Console.WriteLine("Expected to fail");

            res = registrationTests("lina@ gmail.com", "Dolphin5"); //supposed to fail, have space
            Console.WriteLine("Test " + testCounter++ + " " + res);


            Console.WriteLine("Expected to fail");

            res = registrationTests("lina@gmail .com", "Dolphin5"); //supposed to fail, have space
            Console.WriteLine("Test " + testCounter++ + " " + res);


            Console.WriteLine("Expected to fail");

            res = registrationTests("lina@gmail. com", "Dolphin5"); //supposed to fail, have space
            Console.WriteLine("Test " + testCounter++ + " " + res);


            Console.WriteLine("Expected to pass");

            res = registrationTests("lina@gmail.com", "Dolphin5");
            Console.WriteLine("Test " + testCounter++ + " " + res);


            Console.WriteLine("Expected to fail");

            res = registrationTests("lina@gmail.com", "Rainbows59");
            Console.WriteLine("Test " + testCounter++ + " " + res);


            Console.WriteLine("Expected to fail");

            res = registrationTests("Lina@gmail.com", "Rainbows59");
            Console.WriteLine("Test " + testCounter++ + " " + res);
        }





        ///<summary>
        ///This function tests Functional Requirement 8: logging in a valid user
        /// </summary>
        public string loginTest(string email, string password)
        {
            return user.login(email, password);
        }





        public void loginTest() {
            string res;
            logoutTest("lina@gmail.com");

            Console.WriteLine("Expected to fail");

            res = loginTest("lina", "Dolphin5");
            Console.WriteLine("Testing 18 " + res);


            Console.WriteLine("Expected to fail");

            res = loginTest("lina@gmail.com", "Rainbows6"); 
            Console.WriteLine("Testing 19 " + res);


            Console.WriteLine("Expected to fail");

            res = loginTest("lina@gmail.com", null);
            Console.WriteLine("Testing 19 " + res);


            Console.WriteLine("Expected to fail");

            res = loginTest("lina@gmail.com", "");
            Console.WriteLine("Testing 19 " + res);


            Console.WriteLine("Expected to fail");

            res = loginTest("lina@gmail.com", "dolphin5");
            Console.WriteLine("Testing 20 " + res);


            Console.WriteLine("Expected to fail");

            res = loginTest("lina@gmail.com", "DOLPHIN5");
            Console.WriteLine("Testing 21 " + res);


            Console.WriteLine("Expected to fail");

            res = loginTest(null, "Dolphin5");
            Console.WriteLine("Testing 21 " + res);


            Console.WriteLine("Expected to fail");

            res = loginTest("", "Dolphin5");
            Console.WriteLine("Testing 21 " + res);


            Console.WriteLine("Expected to fail");

            res = loginTest("galco@gmail.com", "Rhetorical5");
            Console.WriteLine("Testing 22 " + res);

            logoutTest("lina@gmail.com");
            Console.WriteLine("Expected to work");

            res = loginTest("lina@gmail.com", "Dolphin5"); //supposed to work
            Console.WriteLine("Testing 23 " + res);


            Console.WriteLine("Expected to fail");

            res = loginTest("lina@gmail.com", "Dolphin5"); //supposed to fail
            Console.WriteLine("Testing 23 " + res);

            logoutTest("lina@gmail.com");


            Console.WriteLine("Expected to work");

            res = loginTest("LINA@GMAIL.COM", "Dolphin5"); //supposed to work
            Console.WriteLine("Testing 23 " + res);
        }





        ///<summary>
        ///This function tests Functional Requirement 8: logging out of the current user
        /// </summary>
        public string logoutTest(string email)
        {
            return user.logout(email);
        }
        




        public void logoutTest()
        {
            string res;




            Console.WriteLine("Expected to fail");

            res = logoutTest("robots@gmail.com");
            Console.WriteLine("Testing 24 " + res);


            Console.WriteLine("Expected to work");

            res = logoutTest("lina@gmail.com"); //supposed to successfully logout
            Console.WriteLine("Testing 25 " + res);


            Console.WriteLine("Expected to fail");

            res = logoutTest("lina@gmail.com"); //supposed to fail
            Console.WriteLine("Testing 26 " + res);
        }
    }
}
