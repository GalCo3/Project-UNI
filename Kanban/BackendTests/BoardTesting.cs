using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntroSE.Kanban.Backend.ServiceLayer;
using IntroSE.Kanban.Backend.DataAccessLayer;
using System.Collections;


namespace BackendTests
{
    class BoardTesting
    {
        private BoardService board;
        private UserService user;


        private BoardDalController boardDalController;
        private UserBoardDalController userBoardDalController;
        private TaskDalController taskDalController;
        private BoardIdCounterDalController boardIdCounterDalController;
        private TaskCounterDalController taskCounterDalController;
        private UserDalController userDalController;

        public BoardTesting(UserService us, BoardService b)
        {
            board = b;
            user = us;

        }



        public  void runTests()
        {
            Console.WriteLine("------------------Testing Board and Task-------------------------------------------");

            Console.WriteLine("------------------Testing AddBoardTests-------------------------------------------");
            addBoardTests();

            Console.WriteLine("------------------Testing RemoveBoardTests-------------------------------------------");
            removeBoardTest();

            Console.WriteLine("------------------Testing ListProgressTasksTest-------------------------------------------");
            listProgressTasksTest();

            Console.WriteLine("------------------Testing setCapacityTest-------------------------------------------");
            setCapacityTest();

            Console.WriteLine("------------------Testing getCapacityTest-------------------------------------------");
            getCapacityTest();

            Console.WriteLine("------------------Testing addTaskTest-------------------------------------------");
            addTaskTest();

            Console.WriteLine("------------------Testing moveTaskTest-------------------------------------------");
            moveTaskTest();

            Console.WriteLine("------------------Testing editTaskTests-------------------------------------------");
            editTests();

            Console.WriteLine("------------------Testing assignTaskTests-------------------------------------------");
            assignTaskTests();

            Console.WriteLine("------------------Testing JoinBoard-------------------------------------------");
            joinBoardTests();

            Console.WriteLine("------------------Testing LeaveBoard-------------------------------------------");
            leaveBoardTests();

            Console.WriteLine("------------------Testing TransferBoardOwner-------------------------------------------");
            transferOwnershipTests();

            Console.WriteLine("------------------Testing LoadData-------------------------------------------");
            //LoadDataTesting();

            Console.WriteLine("------------------Testing RemoveData-------------------------------------------");
            RemoveDataTesting();

        }



        ///<summary>
        ///This function tests the setCapacity method.
        /// </summary>
        public void addBoardTests()
        {
            string res;
            //testing addBoard
            user.login("lina@gmail.com", "Dolphin5");

            Console.WriteLine("Expected to fail");

            res = board.addBoard("Project1", "galco@gmail.com"); //failed, email not logged in
            Console.WriteLine("Test 1 " + res);



            Console.WriteLine("Expected to work");

            res = board.addBoard("Project1", "lina@gmail.com"); //successful 0
            Console.WriteLine("Test 2 " + res);


            Console.WriteLine("Expected to fail");

            res = board.addBoard("   ", "lina@gmail.com"); //failed cant have board name that is empty
            Console.WriteLine("Test 3 " + res);


            Console.WriteLine("Expected to fail");

            res = board.addBoard("", "lina@gmail.com"); //board with empty name
            Console.WriteLine("Test 4 " + res);


            Console.WriteLine("Expected to fail");

            res = board.addBoard(null, "lina@gmail.com"); //board null
            Console.WriteLine("Test 5 " + res);


            Console.WriteLine("Expected to fail");

            res = board.addBoard("Project1", null); //null email
            Console.WriteLine("Test 6 " + res);


            Console.WriteLine("Expected to fail");

            res = board.addBoard("Project1", "lina@gmail.com");//cant add same board with same name
            Console.WriteLine("Test 7 " + res);


            Console.WriteLine("Expected to fail");

            res = board.addBoard("Project1", "pp@gmail.com");//email not registered yet
            Console.WriteLine("Test 8 " + res);


            user.createNewUser("pp@gmail.com", "RacesTest2");


            Console.WriteLine("Expected to work");

            res = board.addBoard("Project1", "pp@gmail.com"); //successfull 1
            Console.WriteLine("Test 9 " + res);

            user.logout("lina@gmail.com");


            Console.WriteLine("Expected to fail");

            res = board.addBoard("Pizzas", "lina@gmail.com"); //attempting to add board to logged out user
            Console.WriteLine("Test 10 " + res);


            Console.WriteLine("Expected to fail");

            res = board.addBoard("Project1", "lina@gmail.com"); //attempting to add same board to logged out user
            Console.WriteLine("Test 11 " + res);

            user.login("lina@gmail.com", "Dolphin5");


            Console.WriteLine("Expected to work");

            res = board.addBoard("Pizzas", "lina@gmail.com"); //successful 2
            Console.WriteLine("Test 12 " + res);

            user.logout("pp@gmail.com");

            user.logout("lina@gmail.com");

        }


        ///<summary>
        ///This function tests the setCapacity method.
        /// </summary>
        public void removeBoardTest()
        {
            string res;
            //testing remove board
            user.login("lina@gmail.com", "Dolphin5");


            Console.WriteLine("Expected to fail");

            res = board.removeBoard("galco@gmail.com", "Project1" ); //failed, not right email
            Console.WriteLine("Test 13 " + res);


            Console.WriteLine("Expected to fail");

            res = board.removeBoard("lina@gmail.com","PizzaMaking"); //failed, no such board
            Console.WriteLine("Test 14 " + res);


            board.addBoard("PizzaMaking", "lina@gmail.com"); //3

            Console.WriteLine("Expected to fail");

            res = board.removeBoard("lina@gmail.com","NutellaRecipe"); //failed no such board
            Console.WriteLine("Test 15 " + res);


            user.logout("lina@gmail.com");


            Console.WriteLine("Expected to fail");

            res = board.removeBoard("lina@gmail.com", "PizzaMaking"); //failed
            Console.WriteLine("Test 16 " + res);


            user.login("lina@gmail.com", "Dolphin5");


            Console.WriteLine("Expected to fail");

            res = board.removeBoard(null, "PizzaMaking"); //failed
            Console.WriteLine("Test 17 " + res);


            //to make sure we can't remove a board by a member
            user.createNewUser("user111@gmail.com","Password1");
            board.JoinBoard("user111@gmail.com", 3);

            Console.WriteLine("Expected to fail");

            res = board.removeBoard("user111@gmail.com", "PizzaMaking");
            Console.WriteLine("Test 18 " + res);


            Console.WriteLine("Expected to work");

            res = board.removeBoard("lina@gmail.com","PizzaMaking"); //successful
            Console.WriteLine("Test 19 " + res);


            Console.WriteLine("Expected to fail");

            res = board.removeBoard("lina@gmail.com", "PizzaMaking"); //failed
            Console.WriteLine("Test 20 " + res);


            Console.WriteLine("Expected to fail");

            res = board.removeBoard("lina@gmail.com", null); //failed
            Console.WriteLine("Test 21 " + res);


            //to make sure that we properly removed it, which means we can add the same board name again.
            Console.WriteLine("Expected to work");
            board.addBoard("PizzaMaking", "lina@gmail.com"); //4
            res = board.removeBoard("lina@gmail.com","PizzaMaking");
            Console.WriteLine("Test 22 " + res);


            Console.WriteLine("Expected to work");

            res = board.removeBoard("lina@gmail.com", "Pizzas"); //successful
            Console.WriteLine("Test 23 " + res);


            Console.WriteLine("Expected to work");

            res = board.removeBoard("lina@gmail.com","Project1"); //successful
            Console.WriteLine("Test 24 " + res);


            Console.WriteLine("Expected to fail");

            res = board.removeBoard("lina@gmail.com", "   "); //board with empty name
            Console.WriteLine("Test 25 " + res);


            Console.WriteLine("Expected to fail");

            res = board.removeBoard("lina@gmail.com", ""); //board with empty name
            Console.WriteLine("Test 26 " + res);

        }


        ///<summary>
        ///This function tests the setCapacity method.
        /// </summary>
        public void listProgressTasksTest() {

            string res;

            //testing listofprogresstests
            user.createNewUser("poopoo@gmail.com", "Dababy5");


            Console.WriteLine("Expected to be empty");

            res = board.ListProgressTasks("poopoo@gmail.com"); //trying without any boards
            Console.WriteLine("Test 27 " + res);


            Console.WriteLine("Expected to fail");

            res = board.ListProgressTasks(null); //trying with null user
            Console.WriteLine("Test 28 " + res);


            board.addBoard("Assignments", "poopoo@gmail.com"); //5

            Console.WriteLine("Expected to be empty");

            res = board.ListProgressTasks("poopoo@gmail.com"); //trying with an empty board
            Console.WriteLine("Test 29 " + res);


            Console.WriteLine("Expected to fail");

            res = board.ListProgressTasks("butterflies@gmail.com"); //trying with a non existent user
            Console.WriteLine("Test 30 " + res);


            board.addTask("poopoo@gmail.com", "Assignments", "OOP assignment 1", "Assignment about writing a calculator", new DateTime(2025, 12, 25));
            board.addTask("poopoo@gmail.com", "Assignments", "Data Structures 2", "Assignment about building AVL trees", new DateTime(2025, 12, 25));
            board.AssignTask("poopoo@gmail.com", "Assignments", 0, 0, "poopoo@gmail.com");
            board.AssignTask("poopoo@gmail.com", "Assignments", 0, 1, "poopoo@gmail.com");
            board.moveTask("poopoo@gmail.com", "Assignments", 0, 0);
            board.moveTask("poopoo@gmail.com", "Assignments", 0, 1);


            board.addBoard("Exams", "poopoo@gmail.com"); //6
            board.addTask("poopoo@gmail.com", "Exams", "Combinatorics Midterm", "Assignment about writing a calculator", new DateTime(2025, 12, 25));
            board.addTask("poopoo@gmail.com", "Exams", "Calculus Midterm", "Assignment about writing a calculator", new DateTime(2025, 12, 25));
            board.addTask("poopoo@gmail.com", "Exams", "Historical Events of the 21st Century", "Exam to memorize alot of events", new DateTime(2025, 12, 25));
            board.addTask("poopoo@gmail.com", "Exams", "Driving Exam", "Exam to drive", new DateTime(2022, 12, 25));

            board.AssignTask("poopoo@gmail.com", "Exams", 0, 0, "poopoo@gmail.com");
            board.AssignTask("poopoo@gmail.com", "Exams", 0, 1, "poopoo@gmail.com");
            board.AssignTask("poopoo@gmail.com", "Exams", 0, 2, "poopoo@gmail.com");

            board.moveTask("poopoo@gmail.com", "Exams", 0, 0);
            board.moveTask("poopoo@gmail.com", "Exams", 0, 1);
            board.moveTask("poopoo@gmail.com", "Exams", 0, 2);

            //should still be empty since tasks are not assigned to email
            Console.WriteLine("Expected to be empty");

            res = board.ListProgressTasks("poopoo@gmail.com"); //trying with two boards with tasks
            Console.WriteLine("Test 31 " + res);



            Console.WriteLine("Expected to show tasks");

            res = board.ListProgressTasks("poopoo@gmail.com"); //trying with two boards with tasks
            Console.WriteLine("Test 32 " + res);


            board.moveTask("poopoo@gmail.com", "Assignments", 1, 0);
            board.moveTask("poopoo@gmail.com", "Assignments", 1, 1);
            board.moveTask("poopoo@gmail.com", "Exams", 1, 0);
            board.moveTask("poopoo@gmail.com", "Exams", 1, 1);
            board.moveTask("poopoo@gmail.com", "Exams", 1, 2);


            //5 not moved
            Console.WriteLine("Expected to empty");

            res = board.ListProgressTasks("poopoo@gmail.com"); //should be failed or return empty since inProgress should be empty now
            Console.WriteLine("Test 33 " + res);


            Console.WriteLine("Expected to fail");

            user.logout("poopoo@gmail.com");
            res = board.ListProgressTasks("poopoo@gmail.com"); //should be failed since email is logged out
            Console.WriteLine("Test 34 " + res);
        }


        ///<summary>
        ///This function tests the setCapacity method.
        /// </summary>
        public void setCapacityTest()
        {

            string res;
            // testing setCapacity

            user.createNewUser("stinkyqueen@gmail.com", "Monkeys1");
            board.addBoard("Board0", "stinkyqueen@gmail.com"); //7


            //email problems

            Console.WriteLine("Expected to fail");

            res = board.setCapacity(null, "Board0", 2, 0); //failed, null
            Console.WriteLine("Test 35 " + res);


            Console.WriteLine("Expected to fail");

            res = board.setCapacity(null, null, 2, 0); //failed, null
            Console.WriteLine("Test 36 " + res);


            Console.WriteLine("Expected to fail");

            res = board.setCapacity("fifthharmoy@gmail.com", "Board0", 2, 0); //failed email doesnt exist
            Console.WriteLine("Test 37 " + res);


            Console.WriteLine("Expected to fail");

            res = board.setCapacity("", "Board0", 2, 0); //empty user
            Console.WriteLine("Test 38 " + res);


            user.logout("stinkyqueen@gmail.com");

            Console.WriteLine("Expected to fail");

            res = board.setCapacity("stinkyqueen@gmail.com", "Board0", 2, 0); //user not logged in
            Console.WriteLine("Test 39 " + res);


            user.login("stinkyqueen@gmail.com", "Monkeys1");

            //board name problems

            Console.WriteLine("Expected to fail");

            res = board.setCapacity("stinkyqueen@gmail.com", "Board1", 2, 0); //wrong board name
            Console.WriteLine("Test 40 " + res);


            Console.WriteLine("Expected to fail");

            res = board.setCapacity("stinkyqueen@gmail.com", "", 2, 0); //wrong board name
            Console.WriteLine("Test 41 " + res);


            Console.WriteLine("Expected to fail");

            res = board.setCapacity("stinkyqueen@gmail.com", null, 2, 0); //board null
            Console.WriteLine("Test 42 " + res);


            //limit problems

            Console.WriteLine("Expected to fail");

            res = board.setCapacity("stinkyqueen@gmail.com", "Board0", -2, 0); //out of bounds
            Console.WriteLine("Test 43 " + res);


            //columnIndex problems

            Console.WriteLine("Expected to fail");

            res = board.setCapacity("stinkyqueen@gmail.com", "Board0", 3, -1); //out of bounds
            Console.WriteLine("Test 44 " + res);


            Console.WriteLine("Expected to fail");

            res = board.setCapacity("stinkyqueen@gmail.com", "Board0", 3, 3); //out of bounds
            Console.WriteLine("Test 45 " + res);


            //supposed to work + changing around and playing with adding tasks/ moving tasks 

            Console.WriteLine("Expected to work");

            res = board.setCapacity("stinkyqueen@gmail.com", "Board0", 3, 0); //works
            Console.WriteLine("Test 46 " + res + board.getCapacity("stinkyqueen@gmail.com", "Board0", 0));


            Console.WriteLine("Expected to work");

            res = board.setCapacity("stinkyqueen@gmail.com", "Board0", 6, 1); //works
            Console.WriteLine("Test 47 " + res + board.getCapacity("stinkyqueen@gmail.com", "Board0", 1));


            Console.WriteLine("Expected to work");

            res = board.setCapacity("stinkyqueen@gmail.com", "Board0", 6, 2); //works
            Console.WriteLine("Test 48 " + res + board.getCapacity("stinkyqueen@gmail.com", "Board0", 2));


            board.addTask("stinkyqueen@gmail.com", "Board0", "oreos", "black oreos", new DateTime(2025, 12, 25));
            board.addTask("stinkyqueen@gmail.com", "Board0", "oreos2", "White oreos", new DateTime(2025, 12, 25));
            board.addTask("stinkyqueen@gmail.com", "Board0", "oreos3", "grey oreos", new DateTime(2025, 12, 25));


            Console.WriteLine("Expected to fail");

            res = board.setCapacity("stinkyqueen@gmail.com", "Board0", 0, 0); //not work, supposed to return error
            Console.WriteLine("Test 49 " + res + board.getCapacity("stinkyqueen@gmail.com", "Board0", 0));


            Console.WriteLine("Expected to work");

            res = board.setCapacity("stinkyqueen@gmail.com", "Board0", -1, 0); //works
            Console.WriteLine("Test 50 " + res + board.getCapacity("stinkyqueen@gmail.com", "Board0", 0));


            Console.WriteLine("Expected to work");

            res = board.setCapacity("stinkyqueen@gmail.com", "Board0", 6, 0); // works
            Console.WriteLine("Test 51 " + res + board.getCapacity("stinkyqueen@gmail.com", "Board0", 0));


            board.addTask("stinkyqueen@gmail.com", "Board0", "oreos", "purple oreos", new DateTime(2025, 12, 25));
            board.addTask("stinkyqueen@gmail.com", "Board0", "oreos2", "blue oreos", new DateTime(2025, 12, 25));
            board.addTask("stinkyqueen@gmail.com", "Board0", "oreos3", "red oreos", new DateTime(2025, 12, 25));
            

            Console.WriteLine("Expected to fail");

            res = board.setCapacity("stinkyqueen@gmail.com", "Board0", 5, 0); //not work, supposed to return error
            Console.WriteLine("Test 52 " + res + board.getCapacity("stinkyqueen@gmail.com", "Board0", 0));


            Console.WriteLine("Expected to work");

            res = board.setCapacity("stinkyqueen@gmail.com", "Board0", -1, 0); //works
            Console.WriteLine("Test 53 " + res + board.getCapacity("stinkyqueen@gmail.com", "Board0", 0));

            board.AssignTask("stinkyqueen@gmail.com", "Board0", 0, 0, "stinkyqueen@gmail.com");
            board.AssignTask("stinkyqueen@gmail.com", "Board0", 0, 1, "stinkyqueen@gmail.com");
            board.AssignTask("stinkyqueen@gmail.com", "Board0", 0, 2, "stinkyqueen@gmail.com");
            board.AssignTask("stinkyqueen@gmail.com", "Board0", 0, 3, "stinkyqueen@gmail.com");
            board.AssignTask("stinkyqueen@gmail.com", "Board0", 0, 4, "stinkyqueen@gmail.com");
            board.AssignTask("stinkyqueen@gmail.com", "Board0", 0, 5, "stinkyqueen@gmail.com");

            board.moveTask("stinkyqueen@gmail.com", "Board0", 0, 0);
            board.moveTask("stinkyqueen@gmail.com", "Board0", 0, 1);
            board.moveTask("stinkyqueen@gmail.com", "Board0", 0, 2);
            board.moveTask("stinkyqueen@gmail.com", "Board0", 0, 3);
            board.moveTask("stinkyqueen@gmail.com", "Board0", 0, 4);
            board.moveTask("stinkyqueen@gmail.com", "Board0", 0, 5);


            Console.WriteLine("Expected to work");

            res = board.setCapacity("stinkyqueen@gmail.com", "Board0", 0, 0); //works since no tasks in backlog anymore
            Console.WriteLine("Test 54 " + res + board.getCapacity("stinkyqueen@gmail.com", "Board0", 0));


            Console.WriteLine("Expected to fail");

            res = board.setCapacity("stinkyqueen@gmail.com", "Board0", 0, 1); //not work, supposed to return error
            Console.WriteLine("Test 55 " + res + board.getCapacity("stinkyqueen@gmail.com", "Board0", 1));


            Console.WriteLine("Expected to work");

            res = board.setCapacity("stinkyqueen@gmail.com", "Board0", -1, 1); //works
            Console.WriteLine("Test 56 " + res + board.getCapacity("stinkyqueen@gmail.com", "Board0", 1));


            Console.WriteLine("Expected to work");

            res = board.setCapacity("stinkyqueen@gmail.com", "Board0", 6, 1); // works
            Console.WriteLine("Test 57 " + res + board.getCapacity("stinkyqueen@gmail.com", "Board0", 1));


            Console.WriteLine("Expected to fail");

            res = board.setCapacity("stinkyqueen@gmail.com", "Board0", 5, 1); //not work, supposed to return error
            Console.WriteLine("Test 58 " + res + board.getCapacity("stinkyqueen@gmail.com", "Board0", 1));


            board.moveTask("stinkyqueen@gmail.com", "Board0", 1, 0);
            board.moveTask("stinkyqueen@gmail.com", "Board0", 1, 1);
            board.moveTask("stinkyqueen@gmail.com", "Board0", 1, 2);
            board.moveTask("stinkyqueen@gmail.com", "Board0", 1, 3);
            board.moveTask("stinkyqueen@gmail.com", "Board0", 1, 4);
            board.moveTask("stinkyqueen@gmail.com", "Board0", 1, 5);


            Console.WriteLine("Expected to work");

            res = board.setCapacity("stinkyqueen@gmail.com", "Board0", 0, 1); //works since no tasks in backlog anymore
            Console.WriteLine("Test 59 " + res + board.getCapacity("stinkyqueen@gmail.com", "Board0", 1));


            Console.WriteLine("Expected to fail");

            res = board.setCapacity("stinkyqueen@gmail.com", "Board0", 0, 2); //not work, supposed to return error
            Console.WriteLine("Test 60 " + res + board.getCapacity("stinkyqueen@gmail.com", "Board0", 2));


            Console.WriteLine("Expected to work");

            res = board.setCapacity("stinkyqueen@gmail.com", "Board0", -1, 2); //works
            Console.WriteLine("Test 61 " + res + board.getCapacity("stinkyqueen@gmail.com", "Board0", 2));


            Console.WriteLine("Expected to work");

            res = board.setCapacity("stinkyqueen@gmail.com", "Board0", 6, 2); // works
            Console.WriteLine("Test 62 " + res + board.getCapacity("stinkyqueen@gmail.com", "Board0", 2));


            Console.WriteLine("Expected to fail");

            res = board.setCapacity("stinkyqueen@gmail.com", "Board0", 5, 2); //not work, supposed to return error
            Console.WriteLine("Test 63 " + res + board.getCapacity("stinkyqueen@gmail.com", "Board0", 2));

            //validating that non-members cant setCapacity and members can
            user.createNewUser("fifthharmony@gmail.com", "Password1");

            Console.WriteLine("Expected to fail");

            res = board.setCapacity("fifthharmony@gmail.com", "Board0",2 , 0); //failed email isn't member of board
            Console.WriteLine("Test 64 " + res);


            board.JoinBoard("fifthharmony@gmail.com",7);

            Console.WriteLine("Expected to work");

            res = board.setCapacity("fifthharmony@gmail.com", "Board0", 100, 0); //works since email is now a board member
            Console.WriteLine("Test 65 " + res);
        }


        ///<summary>
        ///This function tests the getCapacity method.
        /// </summary>
        public void getCapacityTest() {

            string res;
            user.createNewUser("user33@gmail.com", "Ilovemonkey2");
            user.login("user33@gmail.com", "Ilovemonkey2");
            board.addBoard("cars", "user33@gmail.com"); //8

            //email problems

            Console.WriteLine("Expected to fail");

            res = board.getCapacity("user63732@gmail.com", "cars", 0);// fail- email doesnt exists
            Console.WriteLine("Test 66 " + res);


            Console.WriteLine("Expected to fail");

            res = board.getCapacity("", "cars", 0); //supposed to be error since the email is invalid
            Console.WriteLine("Test 67 " + res);


            Console.WriteLine("Expected to fail");

            res = board.getCapacity(null, "cars", 0); //supposed to be error since the email is null
            Console.WriteLine("Test 68 " + res);


            user.logout("user33@gmail.com");

            Console.WriteLine("Expected to fail");

            res = board.getCapacity("user33@gmail.com", "cars", 0); //supposed to be error since the email is logged out
            Console.WriteLine("Test 69 " + res);


            user.login("user33@gmail.com", "Ilovemonkey2");


            //board name problems

            Console.WriteLine("Expected to fail");

            res = board.getCapacity("user33@gmail.com", "Project9", 0); ////supposed to be error since the board name doesnt exists
            Console.WriteLine("Test 70 " + res);


            Console.WriteLine("Expected to fail");

            res = board.getCapacity("user33@gmail.com", null, 0); ////supposed to be error since the board name is null
            Console.WriteLine("Test 71 " + res);


            //column index problems

            Console.WriteLine("Expected to fail");

            res = board.getCapacity("user33@gmail.com", "cars", -1); ////supposed to be error since columnindex is wrong
            Console.WriteLine("Test 72 " + res);


            Console.WriteLine("Expected to fail");

            res = board.getCapacity("user33@gmail.com", "cars", 3); ////supposed to be error since columnindex is wrong
            Console.WriteLine("Test 73 " + res);


            //works

            Console.WriteLine("Expected to work");

            res = board.getCapacity("user33@gmail.com", "cars", 0); ////supposed to return -1
            Console.WriteLine("Test 74 " + res);


            Console.WriteLine("Expected to work");

            res = board.getCapacity("user33@gmail.com", "cars", 1); ////supposed to return -1
            Console.WriteLine("Test 75 " + res);


            Console.WriteLine("Expected to work");

            res = board.getCapacity("user33@gmail.com", "cars", 2); ////supposed to return -1
            Console.WriteLine("Test 76 " + res);


            board.setCapacity("user33@gmail.com", "cars", 5, 0);

            board.setCapacity("user33@gmail.com", "cars", 10, 1);

            board.setCapacity("user33@gmail.com", "cars", 15, 2);


            Console.WriteLine("Expected to work");

            res = board.getCapacity("user33@gmail.com", "cars", 0); ////supposed to return 5
            Console.WriteLine("Test 77 " + res);


            Console.WriteLine("Expected to work");

            res = board.getCapacity("user33@gmail.com", "cars", 1); ////supposed to return 10
            Console.WriteLine("Test 78 " + res);


            Console.WriteLine("Expected to work");

            res = board.getCapacity("user33@gmail.com", "cars", 2); ////supposed to return 15
            Console.WriteLine("Test 79 " + res);


            board.setCapacity("user33@gmail.com", "cars", -1, 0);

            board.setCapacity("user33@gmail.com", "cars", -1, 1);

            board.setCapacity("user33@gmail.com", "cars", 0, 2);


            Console.WriteLine("Expected to work");

            res = board.getCapacity("user33@gmail.com", "cars", 0); ////supposed to -1
            Console.WriteLine("Test 80 " + res);


            Console.WriteLine("Expected to work");

            res = board.getCapacity("user33@gmail.com", "cars", 1); ////supposed to -1
            Console.WriteLine("Test 81 " + res);


            Console.WriteLine("Expected to work");

            res = board.getCapacity("user33@gmail.com", "cars", 2); ////supposed to 0
            Console.WriteLine("Test 82 " + res);


            board.addTask("user33@gmail.com", "cars", "maserati", "vroom vroom", new DateTime(2025, 12, 25));
            board.addTask("user33@gmail.com", "cars", "ferrari", "vroom vroom", new DateTime(2025, 12, 25));
            board.addTask("user33@gmail.com", "cars", "lambo", "vroom vroom", new DateTime(2025, 12, 25));
            board.setCapacity("user33@gmail.com", "cars", 5, 0);


            Console.WriteLine("Expected to work");

            res = board.getCapacity("user33@gmail.com", "cars", 0); ////supposed to return 5
            Console.WriteLine("Test 83 " + res);


            board.setCapacity("user33@gmail.com", "cars", 0, 0);


            Console.WriteLine("Expected to work");

            res = board.getCapacity("user33@gmail.com", "cars", 0); ////supposed to return 5
            Console.WriteLine("Test 84 " + res);

        }


        ///<summary>
        ///This function tests Functional Requirement 12: Adding a new task to the backlog column.
        /// </summary>
        public void addTaskTest() {
            string res;
            user.createNewUser("bonkey@gmail.com", "Ilovemoney2");
            board.addBoard("Board1", "bonkey@gmail.com"); //9


            Console.WriteLine("Expected to fail");

            res = board.addTask("tamir@gmail.com", "Board1", "", "description 1", new DateTime(2025, 5, 26)); //wrong email
            Console.WriteLine("Test 85 " + res);


            Console.WriteLine("Expected to fail");

            res = board.addTask(null, "Board1", "task1", "description 1", new DateTime(2025, 5, 26)); //null email
            Console.WriteLine("Test 86 " + res);


            Console.WriteLine("Expected to fail");

            res = board.addTask("bonkey@gmail.com", "", "task", "description 1", new DateTime(2025, 5, 26)); //boardname emoty
            Console.WriteLine("Test 87 " + res);


            Console.WriteLine("Expected to fail");

            res = board.addTask("bonkey@gmail.com", null, "task", "description 1", new DateTime(2025, 5, 26)); //boardname null
            Console.WriteLine("Test 88 " + res);


            Console.WriteLine("Expected to fail");

            res = board.addTask("bonkey@gmail.com", "Board1", "", "description 1", new DateTime(2025, 5, 26)); //task empty
            Console.WriteLine("Test 89 " + res);


            Console.WriteLine("Expected to fail");

            res = board.addTask("bonkey@gmail.com", "Board1", null, "description 1", new DateTime(2025, 5, 26)); //task null
            Console.WriteLine("Test 90 " + res);


            Console.WriteLine("Expected to fail");

            res = board.addTask("bonkey@gmail.com", "Board1", "llllllllllllllllllllllllllllllllllllllllllllllllllllllllllllll", "description 1", new DateTime(2025, 5, 26)); //more  than 50
            Console.WriteLine("Test 91 " + res);


            Console.WriteLine("Expected to work");

            res = board.addTask("bonkey@gmail.com", "Board1", "abcdefghijklmnopqrstuvwxyabcdefghijklmnopqrstuvwxy", "description 1", new DateTime(2025, 7, 26)); //works
            Console.WriteLine("Test 92 " + res);


            Console.WriteLine("Expected to fail");

            res = board.addTask("bonkey@gmail.com", "Board1", "abcdefghijklmnopqrstuvwxyabcdefghijklmnopqrstuvwxyz", "description 1", new DateTime(2025, 5, 26)); //more than 50
            Console.WriteLine("Test 93 " + res);


            Console.WriteLine("Expected to fail");

            res = board.addTask("bonkey@gmail.com", "Board1", "task1", null, new DateTime(2025, 5, 26)); //description null
            Console.WriteLine("Test 94 " + res);


            Console.WriteLine("Expected to fail");

            res = board.addTask("bonkey@gmail.com", "Board1", "task1", "abcdefghijklmnopqrstuvwxyabcdefghijklmnopqrstuvwxyabcdefghijklmnopqrstuvwxyabcdefghijklmnopqrstuvwxyabcdefghijklmnopqrstuvwxyabcdefghijklmnopqrstuvwxyabcdefghijklmnopqrstuvwxyabcdefghijklmnopqrstuvwxyabcdefghijklmnopqrstuvwxyabcdefghijklmnopqrstuvwxyabcdefghijklmnopqrstuvwxyabcdefghijklmnopqrstuvwxyz", new DateTime(2025, 5, 26)); //description null
            //doesnt work, more than 300
            Console.WriteLine("Test 91 " + res);


            Console.WriteLine("Expected to work");
           
            res = board.addTask("bonkey@gmail.com", "Board1", "task1", "abcdefghijklmnopqrstuvwxyabcdefghijklmnopqrstuvwxyabcdefghijklmnopqrstuvwxyabcdefghijklmnopqrstuvwxyabcdefghijklmnopqrstuvwxyabcdefghijklmnopqrstuvwxyabcdefghijklmnopqrstuvwxyabcdefghijklmnopqrstuvwxyabcdefghijklmnopqrstuvwxyabcdefghijklmnopqrstuvwxyabcdefghijklmnopqrstuvwxyabcdefghijklmnopqrstuvwxy", new DateTime(2025, 5, 26)); 
            //works
            Console.WriteLine("Test 95 " + res);


            Console.WriteLine("Expected to fail");

            res = board.addTask("bonkey@gmail.com", "Board1", "task1", "description 1", new DateTime()); //duedate empty
            Console.WriteLine("Test 96 " + res);


            Console.WriteLine("Expected to fail");

            res = board.addTask("bonkey@gmail.com", "Board1", "task1", "description 1", new DateTime(1300,1,1)); //duedate before creation time
            Console.WriteLine("Test 97 " + res);


            Console.WriteLine("Expected to fail");

            res = board.addTask("bonkey@gmail.com", "Board1", "task1", "description 1", DateTime.Now); //duedate same time as creation time
            Console.WriteLine("Test 98 " + res);


            Console.WriteLine("Expected to fail");

            res = board.addTask("bonkey@gmail.com", "Board1333", "task1", "hello", new DateTime(2025, 5, 26)); //wrong board name
            Console.WriteLine("Test 99 " + res);


            Console.WriteLine("Expected to work");

            res = board.addTask("bonkey@gmail.com", "Board1", "task1", "description 1", new DateTime(2025, 1, 1)); //works
            Console.WriteLine("Test 100 " + res);


            Console.WriteLine("Expected to work");

            res = board.addTask("bonkey@gmail.com", "Board1", "task1", "description 1", new DateTime(2025, 1, 1)); //works
            Console.WriteLine("Test 101 " + res);


            Console.WriteLine("Expected to work");

            res = board.addTask("bonkey@gmail.com", "Board1", "task2", "description 1", new DateTime(2025, 1, 1)); //works
            Console.WriteLine("Test 102 " + res);


            board.setCapacity("bonkey@gmail.com", "Board1", 6, 0);


            Console.WriteLine("Expected to work");

            res = board.addTask("bonkey@gmail.com", "Board1", "task3", "description 1", new DateTime(2025, 1, 1)); //works
            Console.WriteLine("Test 103 " + res + board.getCapacity("bonkey@gmail.com","Board1",0));


            Console.WriteLine("Expected to fail");

            res = board.addTask("bonkey@gmail.com", "Board1", "task4", "description 1", new DateTime(2025, 1, 1)); //not work
            Console.WriteLine("Test 104 " + res);


            board.setCapacity("bonkey@gmail.com", "Board1", -1, 0);


            Console.WriteLine("Expected to work");

            res = board.addTask("bonkey@gmail.com", "Board1", "task5", "description 1", new DateTime(2025, 1, 1)); //work
            Console.WriteLine("Test 105 " + res);


            Console.WriteLine("Expected to work");

            res = board.addTask("bonkey@gmail.com", "Board1", "task6", "description 1", new DateTime(2025, 1, 1)); //work
            Console.WriteLine("Test 106 " + res);


            Console.WriteLine("Expected to work");

            res = board.addTask("bonkey@gmail.com", "Board1", "task7", "description 1", new DateTime(2025, 1, 1)); //work
            Console.WriteLine("Test 107 " + res);


            Console.WriteLine("Expected to work");

            res = board.addTask("bonkey@gmail.com", "Board1", "task8", "description 1", new DateTime(2025, 1, 1)); // work
            Console.WriteLine("Test 108 " + res);


            Console.WriteLine("Expected to work");

            res = board.addTask("bonkey@gmail.com", "Board1", "task9", "description 1", new DateTime(2025, 1, 1)); // work
            Console.WriteLine("Test 109 " + res);


            Console.WriteLine("Expected to work");

            res = board.addTask("bonkey@gmail.com", "Board1", "task10", "description 1", new DateTime(2025, 1, 1)); // work
            Console.WriteLine("Test 110 " + res);


            user.logout("bonkey@gmail.com");

            Console.WriteLine("Expected to fail");

            res = board.addTask("bonkey@gmail.com", "Board1", "task1", "description 1", new DateTime(2025, 1, 1)); //not works
            Console.WriteLine("Test 112 " + res);

            //checking if non-members can add task and if members can
            user.createNewUser("user112@gmail.com", "Password1");

            Console.WriteLine("Expected to fail");

            res = board.addTask("user112@gmail.com", "Board1", "task2", "description 1", new DateTime(2025, 1, 1)); //not works
            Console.WriteLine("Test 113 " + res);


            board.JoinBoard("user112@gmail.com",9);
            Console.WriteLine("Expected to work"); //since user is member

            res = board.addTask("user112@gmail.com", "Board1", "task2", "description 1", new DateTime(2025, 1, 1)); //works
            Console.WriteLine("Test 114 " + res);
        }


        ///<summary>
        ///This function tests Functional Requirement 13: moving a task from one column to the next
        /// </summary>
        public void moveTaskTest() {
            string res;
            user.createNewUser("user7@gmail.com", "Ilovemonkey2");
            user.login("user7@gmail.com", "Ilovemonkey2");
            board.addBoard("Board7", "user7@gmail.com"); //10

            board.addTask("user7@gmail.com", "Board7", "Call tamir", "tell him he is stinky", new DateTime(2025, 1, 1));


            //email errors

            Console.WriteLine("Expected to fail");

            res = board.moveTask("lina5443@gmail.com", "Board7", 0, 0);//supposed to be error since the email doesnt exist
            Console.WriteLine("Test 115 " + res);


            Console.WriteLine("Expected to fail");

            res = board.moveTask("", "Board7", 0, 0);//supposed to be error since the email isnt valid
            Console.WriteLine("Test 116 " + res);


            Console.WriteLine("Expected to fail");

            res = board.moveTask(null, "Board7", 0, 0);//supposed to be error since the email is null
            Console.WriteLine("Test 117 " + res);


            user.logout("user7@gmail.com");

            Console.WriteLine("Expected to fail");

            res = board.moveTask("user7@gmail.com", "Board7", 0, 0);//supposed to be error since the email is logged out
            Console.WriteLine("Test 118 " + res);

            user.login("user7@gmail.com", "Ilovemonkey2");


            //board name problems 

            Console.WriteLine("Expected to fail");

            res = board.moveTask("user7@gmail.com", "Board8", 0, 0);//supposed to be error since the board doesnt exists
            Console.WriteLine("Test 119 " + res);


            Console.WriteLine("Expected to fail");

            res = board.moveTask("user7@gmail.com", null, 0, 0);//supposed to be error since the board is null
            Console.WriteLine("Test 120 " + res);


            // column ordinal problems


            Console.WriteLine("Expected to fail");

            res = board.moveTask("user7@gmail.com", "Board7", 8, 0);//supposed to be error since the columnOrdinal isnt valid
            Console.WriteLine("Test 121 " + res);


            Console.WriteLine("Expected to fail");

            res = board.moveTask("user7@gmail.com", "Board7", -1, 0);//supposed to be error since the columnOrdinal isnt valid
            Console.WriteLine("Test 122 " + res);


            Console.WriteLine("Expected to fail");

            res = board.moveTask("user7@gmail.com", "Board7", 1, 0);//supposed to be error since the columnOrdinal isnt valid
            Console.WriteLine("Test 123 " + res);


            Console.WriteLine("Expected to fail");

            res = board.moveTask("user7@gmail.com", "Board7", 2, 0);//supposed to be error since the columnOrdinal isnt valid
            Console.WriteLine("Test 124 " + res);


            //task id problems

            Console.WriteLine("Expected to fail");

            res = board.moveTask("user7@gmail.com", "Board7", 0, 1);//supposed to be error since the taskId isnt valid
            Console.WriteLine("Test 125 " + res);


            Console.WriteLine("Expected to fail");

            res = board.moveTask("user7@gmail.com", "Board7", 0, -1);//supposed to be error since the taskId isnt valid
            Console.WriteLine("Test 126 " + res);

            //not assignee problems
            Console.WriteLine("Expected to fail");

            res = board.moveTask("user7@gmail.com", "Board7", 0, 0);//supposed to fail!
            Console.WriteLine("Test 127 " + res);


            board.AssignTask("user7@gmail.com", "Board7", 0, 0, "user7@gmail.com");

            //works!
            Console.WriteLine("Expected to work");

            res = board.moveTask("user7@gmail.com", "Board7", 0, 0);//supposed to work!
            Console.WriteLine("Test 128 " + res);


            Console.WriteLine("Expected to fail");

            res = board.moveTask("user7@gmail.com", "Board7", 0, 0);//supposed to be error since the taskId isnt in "backlog" state anymore!
            Console.WriteLine("Test 129 " + res);


            Console.WriteLine("Expected to work");

            res = board.moveTask("user7@gmail.com", "Board7", 1, 0);//supposed to work!
            Console.WriteLine("Test 130 " + res);


            Console.WriteLine("Expected to fail");

            res = board.moveTask("user7@gmail.com", "Board7", 1, 0);//supposed to be error since the taskId isnt in "inprogress" state anymore!
            Console.WriteLine("Test 131 " + res);


            Console.WriteLine("Expected to fail");

            res = board.moveTask("user7@gmail.com", "Board7", 0, 0);//supposed to be error since the taskId isnt in "backlog" state anymore!
            Console.WriteLine("Test 132 " + res);


            Console.WriteLine("Expected to fail");

            res = board.moveTask("user7@gmail.com", "Board7", 2, 0);//supposed to be error since the task in "done" state cannot be moved!
            Console.WriteLine("Test 133 " + res);


            user.logout("user7@gmail.com");
     
        }


        public void editTests()
        {
            string res;
            user.createNewUser("user8@gmail.com", "Lovecats24");
            user.login("user8@gmail.com", "Lovecats24");
            board.addBoard("cat", "user8@gmail.com"); //11
            board.addTask("user8@gmail.com", "cat", "cats cleaning", "see instructions", new DateTime(2025, 12, 1));


            //user problems

            Console.WriteLine("Expected to fail");

            res = board.editTask("user843943@gmail.com", "cat", 0, 0, "cat!", null, null,0); //upposed to be error since email doesnt exists
            Console.WriteLine("Test 134 " + res);


            Console.WriteLine("Expected to fail");

            res = board.editTask("user843943@gmail.com", "cat", 0, 0, null, "hi", null, 1); //upposed to be error since email doesnt exists
            Console.WriteLine("Test 135 " + res);


            Console.WriteLine("Expected to fail");

            res = board.editTask("user843943@gmail.com", "cat", 0, 0, null, null, new DateTime(2023, 12, 4), 2); //upposed to be error since email doesnt exists
            Console.WriteLine("Test 136 " + res);


            Console.WriteLine("Expected to fail");

            res = board.editTask("", "cat", 0, 0, "cat!", null, null,0); //upposed to be error since email isnt valid
            Console.WriteLine("Test 137 " + res);


            Console.WriteLine("Expected to fail");

            res = board.editTask("", "cat", 0, 0, null, "empty", null, 1); //upposed to be error since email isnt valid
            Console.WriteLine("Test 138 " + res);


            Console.WriteLine("Expected to fail");

            res = board.editTask("", "cat", 0, 0, null, null, new DateTime(2023, 12, 4), 2); //upposed to be error since email isnt valid
            Console.WriteLine("Test 139 " + res);


            Console.WriteLine("Expected to fail");

            res = board.editTask(null, "cat", 0, 0, "i really love cats", null, null,0); //upposed to be error since email is null
            Console.WriteLine("Test 140 " + res);


            Console.WriteLine("Expected to fail");

            res = board.editTask(null, "cat", 0, 0, null, "empty", null, 1); //upposed to be error since email is null
            Console.WriteLine("Test 141 " + res);


            Console.WriteLine("Expected to fail");

            res = board.editTask(null, "cat", 0, 0, null, null, new DateTime(2023, 12, 4), 2); //upposed to be error since email is null
            Console.WriteLine("Test 142 " + res);


            user.logout("user8@gmail.com");

            Console.WriteLine("Expected to fail");

            res = board.editTask("user8@gmail.com", "cat", 0, 0, "ello", null, null,0); //upposed to be error since email is logged out
            Console.WriteLine("Test 143 " + res);


            Console.WriteLine("Expected to fail");

            res = board.editTask("user8@gmail.com", "cat", 0, 0, null, "empty", null, 1); //upposed to be error since email is logged out
            Console.WriteLine("Test 144 " + res);


            Console.WriteLine("Expected to fail");

            res = board.editTask("user8@gmail.com", "cat", 0, 0, null, null, new DateTime(2023, 12, 4), 2); //upposed to be error since email is logged out
            Console.WriteLine("Test 145 " + res);

            user.login("user8@gmail.com", "Lovecats24");


            //board name problems 

            Console.WriteLine("Expected to fail");

            res = board.editTask("user8@gmail.com", "cat666", 0, 0, "cat!", null, null,0);//supposed to be error since the board doesnt exists
            Console.WriteLine("Test 146 " + res);


            Console.WriteLine("Expected to fail");

            res = board.editTask("user8@gmail.com", "cat666", 0, 0, null, "sonobitches?", null, 1);//supposed to be error since the board doesnt exists
            Console.WriteLine("Test 147 " + res);


            Console.WriteLine("Expected to fail");

            res = board.editTask("user8@gmail.com", "cat666", 0, 0, null, null, new DateTime(2023, 12, 6), 2);//supposed to be error since the board doesnt exists
            Console.WriteLine("Test 148 " + res);


            Console.WriteLine("Expected to fail");

            res = board.editTask("user8@gmail.com", null, 0, 0, "cat!", null, null,0);//supposed to be error since the board is null
            Console.WriteLine("Test 149 " + res);


            Console.WriteLine("Expected to fail");

            res = board.editTask("user8@gmail.com", null, 0, 0, null, "balls", null, 1);//supposed to be error since the board is null
            Console.WriteLine("Test 150 " + res);


            Console.WriteLine("Expected to fail");

            res = board.editTask("user8@gmail.com", null, 0, 0, null, null, new DateTime(2023, 12, 6), 2);//supposed to be error since the board is null
            Console.WriteLine("Test 151 " + res);


            // column ordinal problems

            Console.WriteLine("Expected to fail");

            res = board.editTask("user8@gmail.com", "cat", -1, 0, "cat!",null, null,0);//supposed to be error since the columnOrdinal isnt valid
            Console.WriteLine("Test 152 " + res);


            Console.WriteLine("Expected to fail");

            res = board.editTask("user8@gmail.com", "cat", -1, 0, null, "empty", null,1);//supposed to be error since the columnOrdinal isnt valid
            Console.WriteLine("Test 153 " + res);


            Console.WriteLine("Expected to fail");

            res = board.editTask("user8@gmail.com", "cat", -1, 0, null, null, new DateTime(2023, 12, 6), 2); //supposed to be error since the columnOrdinal isnt valid
            Console.WriteLine("Test 154 " + res);


            Console.WriteLine("Expected to fail");

            res = board.editTask("user8@gmail.com", "cat", 8, 0, "cat!", null, null,0);//supposed to be error since the columnOrdinal isnt valid
            Console.WriteLine("Test 155 " + res);


            Console.WriteLine("Expected to fail");

            res = board.editTask("user8@gmail.com", "cat", 8, 0, null, "empty", null,1);//supposed to be error since the columnOrdinal isnt valid
            Console.WriteLine("Test 156 " + res);


            Console.WriteLine("Expected to fail");

            res = board.editTask("user8@gmail.com", "cat", 8, 0, null, null, new DateTime(2023, 12, 6), 2);//supposed to be error since the columnOrdinal isnt valid
            Console.WriteLine("Test 157 " + res);


            Console.WriteLine("Expected to fail");

            res = board.editTask("user8@gmail.com", "cat", 1, 0, "cat!", null, null,0);//supposed to be error since the columnOrdinal isnt valid
            Console.WriteLine("Test 158 " + res);


            Console.WriteLine("Expected to fail");

            res = board.editTask("user8@gmail.com", "cat", 1, 0, null, "empty", null,1);//supposed to be error since the columnOrdinal isnt valid
            Console.WriteLine("Test 159 " + res);


            Console.WriteLine("Expected to fail");

            res = board.editTask("user8@gmail.com", "cat", 1, 0, null, null, new DateTime(2023, 12, 6), 2);//supposed to be error since the columnOrdinal isnt valid
            Console.WriteLine("Test 160 " + res);


            Console.WriteLine("Expected to fail");

            res = board.editTask("user8@gmail.com", "cat", 2, 0, "cat!", null, null,0); //supposed to be error since the columnOrdinal isnt valid
            Console.WriteLine("Test 161 " + res);


            Console.WriteLine("Expected to fail");

            res = board.editTask("user8@gmail.com", "cat", 2,0, null, "empty", null,1); //supposed to be error since the columnOrdinal isnt valid
            Console.WriteLine("Test 162 " + res);


            Console.WriteLine("Expected to fail");

            res = board.editTask("user8@gmail.com", "cat", 2, 0, null,null, new DateTime(2023, 12, 6), 2); //supposed to be error since the columnOrdinal isnt valid
            Console.WriteLine("Test 163 " + res);

            //assignee problem
            Console.WriteLine("Expected to fail");

            res = board.editTask("user8@gmail.com", "cat", 0, 0, "breed", null, null, 0); //supposed to be error
            Console.WriteLine("Test 164 " + res);

            board.AssignTask("user8@gmail.com", "cat", 0, 0, "user8@gmail.com");


            //title problem

            Console.WriteLine("Expected to fail");

            res = board.editTask("user8@gmail.com", "cat", 0, 0, "llllllllllllllllllllllllllllllllllllllllllllllllllllllllllllll", null, null, 0); //supposed to be error since the title is invalid
            Console.WriteLine("Test 165 " + res);


            Console.WriteLine("Expected to fail");

            res = board.editTask("user8@gmail.com", "cat", 0, 0, null, null, null, 0); //supposed to be error since the title is invalid
            Console.WriteLine("Test 166 " + res);


            Console.WriteLine("Expected to fail");

            res = board.editTask("user8@gmail.com", "cat", 0, 0, "", null, null, 0); //supposed to be error since the title is invalid
            Console.WriteLine("Test 167 " + res);


            Console.WriteLine("Expected to work");

            res = board.editTask("user8@gmail.com", "cat", 0, 0, "breed", null, null, 0); //supposed to work
            Console.WriteLine("Test 168 " + res);


            //description problem

            Console.WriteLine("Expected to fail");

            res = board.editTask("user8@gmail.com", "cat", 0, 0, null, "llllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllll", null,1); //supposed to be error since the description is invalid
            Console.WriteLine("Test 169 " + res);


            Console.WriteLine("Expected to fail");

            res = board.editTask("user8@gmail.com", "cat", 0, 0, null, null, null, 1); //supposed to be error since the description is invalid
            Console.WriteLine("Test 170 " + res);


            Console.WriteLine("Expected to work");

            res = board.editTask("user8@gmail.com", "cat", 0, 0, null, "find two cats to breed", null, 1); //supposed to work
            Console.WriteLine("Test 171 " + res);


            //duedate problem

            Console.WriteLine("Expected to fail");

            res = board.editTask("user8@gmail.com", "cat", 0, 0, null, null, new DateTime(2022, 5, 15), 2); //supposed to be error since the duedate is invalid
            Console.WriteLine("Test 172 " + res);


            Console.WriteLine("Expected to fail");

            res = board.editTask("user8@gmail.com", "cat", 0, 0, null, null, DateTime.Now, 2); //supposed to be error since the duedate is invalid
            Console.WriteLine("Test 173 " + res);


            Console.WriteLine("Expected to work");

            res = board.editTask("user8@gmail.com", "cat", 0, 0, null, null, new DateTime(2023, 5, 15), 2); //supposed to work
            Console.WriteLine("Test 174 " + res);


            //move task and try edit

            res = board.moveTask("user8@gmail.com", "cat", 0, 0);

            Console.WriteLine("Expected to fail");

            res = board.editTask("user8@gmail.com", "cat", 0, 0, "no cats", null, null, 0); //fail- wrong column index
            Console.WriteLine("Test 175 " + res);


            Console.WriteLine("Expected to fail");

            res = board.editTask("user8@gmail.com", "cat", 0, 0, null, "just no cats", null, 1); //fail- wrong column index
            Console.WriteLine("Test 176 " + res);


            Console.WriteLine("Expected to fail");

            res = board.editTask("user8@gmail.com", "cat", 0, 0, null, null, new DateTime(2024, 5, 15), 2); //fail- wrong column index
            Console.WriteLine("Test 177 " + res);


            //title problem

            Console.WriteLine("Expected to fail");

            res = board.editTask("user8@gmail.com", "cat", 1, 0, "llllllllllllllllllllllllllllllllllllllllllllllllllllllllllllll", null, null, 0); //supposed to be error since the title is invalid
            Console.WriteLine("Test 178 " + res);


            Console.WriteLine("Expected to fail");

            res = board.editTask("user8@gmail.com", "cat", 1, 0, null, null, null, 0); //supposed to be error since the title is invalid
            Console.WriteLine("Test 179 " + res);


            Console.WriteLine("Expected to fail");

            res = board.editTask("user8@gmail.com", "cat", 1, 0, "", null, null, 0); //supposed to be error since the title is invalid
            Console.WriteLine("Test 180 " + res);


            Console.WriteLine("Expected to work");

            res = board.editTask("user8@gmail.com", "cat", 1, 0, "type", null, null, 0); //supposed to work
            Console.WriteLine("Test 181 " + res);


            //description problem

            Console.WriteLine("Expected to fail");

            res = board.editTask("user8@gmail.com", "cat", 1, 0, null, "llllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllll", null, 1); //supposed to be error since the description is invalid
            Console.WriteLine("Test 182 " + res);


            Console.WriteLine("Expected to fail");

            res = board.editTask("user8@gmail.com", "cat", 1, 0, null, null, null, 1); //supposed to be error since the description is invalid
            Console.WriteLine("Test 183 " + res);


            Console.WriteLine("Expected to work");

            res = board.editTask("user8@gmail.com", "cat", 1, 0, null, "scottish fold and brittish manfold", null, 1); //supposed to work
            Console.WriteLine("Test 184 " + res);


            //duedate problem

            Console.WriteLine("Expected to fail");

            res = board.editTask("user8@gmail.com", "cat", 1, 0, null, null, new DateTime(2022, 5, 15), 2); //supposed to be error since the duedate is invalid
            Console.WriteLine("Test 185 " + res);


            Console.WriteLine("Expected to fail");

            res = board.editTask("user8@gmail.com", "cat", 1, 0, null, null, DateTime.Now, 2); //supposed to be error since the duedate is invalid
            Console.WriteLine("Test 186 " + res);


            Console.WriteLine("Expected to work");

            res = board.editTask("user8@gmail.com", "cat", 1, 0, null, null, new DateTime(2023, 5, 15), 2); //supposed to work
            Console.WriteLine("Test 187 " + res);


            //task index problems

            Console.WriteLine("Expected to fail");

            res = board.editTask("user8@gmail.com", "cat", 1, 1, "no cats", null, null, 0); //fail- wrong task index
            Console.WriteLine("Test 188 " + res);


            //move again (now in "done" state) and try to edit (fail)
            res = board.moveTask("user8@gmail.com", "cat", 1, 0);

            Console.WriteLine("Expected to fail");

            res = board.editTask("user8@gmail.com", "cat", 2, 0, "cat?", null, null, 0); //supposed to fail
            Console.WriteLine("Test 189 " + res);


            Console.WriteLine("Expected to fail");

            res = board.editTask("user8@gmail.com", "cat", 2, 0, null, "dog", null, 1); //supposed to fail
            Console.WriteLine("Test 190 " + res);


            Console.WriteLine("Expected to fail");

            res = board.editTask("user8@gmail.com", "cat", 2, 0, null, null, new DateTime(2025, 5, 15), 2); //supposed to fail
            Console.WriteLine("Test 191 " + res);
        }


        public void joinBoardTests()
        {
            string res;
            user.createNewUser("user98@gmail.com", "Tamir12341234");
            board.addBoard("uni", "user98@gmail.com"); //13

            user.createNewUser("bruhman@gmail.com", "Tamir123412311");

            // email problems

            Console.WriteLine("Expected to fail");

            res = board.JoinBoard("user98@gmail.com", 13); // fail - user already owner
            Console.WriteLine("Test 192 " + res);


            Console.WriteLine("Expected to fail");

            res = board.JoinBoard("fff", 13); // fail- email isnt valid
            Console.WriteLine("Test 193 " + res);


            Console.WriteLine("Expected to fail");

            res = board.JoinBoard(null, 13); // fail- email is null
            Console.WriteLine("Test 194 " + res);


            Console.WriteLine("Expected to fail");

            res = board.JoinBoard("doggo@gmail.com", 13); // fail- email doesnt exists
            Console.WriteLine("Test 195 " + res);

            user.logout("bruhman@gmail.com");


            Console.WriteLine("Expected to fail");

            res = board.JoinBoard("bruhman@gmail.com", 13); //supposed to be error since email is logged out
            Console.WriteLine("Test 196 " + res);

            user.login("bruhman@gmail.com", "Tamir123412311");


            //board id problems

            Console.WriteLine("Expected to fail");

            res = board.JoinBoard("bruhman@gmail.com", -13); // fail- board id is negative
            Console.WriteLine("Test 197 " + res);


            Console.WriteLine("Expected to fail");

            res = board.JoinBoard("bruhman@gmail.com", 14); // fail- board id doesnt exists
            Console.WriteLine("Test 198 " + res);


            Console.WriteLine("Expected to fail");

            res = board.JoinBoard("bruhman@gmail.com", 100); // fail- board id doesnt exists
            Console.WriteLine("Test 199 " + res);


            //supposed to work

            Console.WriteLine("Expected to work");

            res = board.JoinBoard("bruhman@gmail.com", 13); // works
            Console.WriteLine("Test 200 " + res);


            Console.WriteLine("Expected to be error");

            res = board.JoinBoard("bruhman@gmail.com", 13); // supposed to return undefined - can't join same board twice.
            Console.WriteLine("Test 201 " + res);


            //adding an additional member to make sure multiple are able to join

            user.createNewUser("manbruh@gmail.com", "Tamir12341");

            Console.WriteLine("Expected to work");

            res = board.JoinBoard("manbruh@gmail.com", 13); // works
            Console.WriteLine("Test 203 " + res);


            user.logout("manbruh@gmail.com");
            user.logout("bruhman@gmail.com");

            //checking if a user can join two boards of similar name
            user.createNewUser("mom@gmail.com", "Password1");

            board.addBoard("uni", "mom@gmail.com");

            Console.WriteLine("Expected to fail");

            res = board.JoinBoard("mom@gmail.com", 13); //cant join board of similar name
            Console.WriteLine("Test 203.5 " + res);
        }

        public void assignTaskTests()
        {
            string res;
            user.createNewUser("user999@gmail.com", "Password1");
            board.addBoard("board", "user999@gmail.com"); //12

            user.createNewUser("user998@gmail.com", "Password1"); //assignee temp

            board.addTask("user999@gmail.com", "board", "Task", "description", new DateTime(2025, 1, 1));

            //supposed to work
            Console.WriteLine("Expected to work:");

            res = board.AssignTask("user999@gmail.com", "board", 0, 0, "user999@gmail.com");
            Console.WriteLine("Test 204 " + res);


            //email problems
            Console.WriteLine("Expected to fail:");

            res = board.AssignTask("", "board", 0, 0, "user998@gmail.com");
            Console.WriteLine("Test 205 " + res);


            Console.WriteLine("Expected to fail:");

            res = board.AssignTask(null, "board", 0, 0, "user998@gmail.com"); //supposed to be error
            Console.WriteLine("Test 206 " + res);


            Console.WriteLine("Expected to fail:");

            res = board.AssignTask("Spikespiegel@gmail.com", "board", 0, 0, "user998@gmail.com"); //supposed to be error
            Console.WriteLine("Test 207 " + res);


            //boardName problems

            Console.WriteLine("Expected to fail:");

            res = board.AssignTask("user999@gmail.com", "Board", 0, 0, "user998@gmail.com"); //supposed to be error
            Console.WriteLine("Test 208 " + res);


            Console.WriteLine("Expected to fail:");

            res = board.AssignTask("user999@gmail.com", null, 0, 0, "user998@gmail.com"); //supposed to be error
            Console.WriteLine("Test 209 " + res);


            Console.WriteLine("Expected to fail:");

            res = board.AssignTask("user999@gmail.com", "", 0, 0, "user998@gmail.com"); //supposed to be error
            Console.WriteLine("Test 210 " + res);


            //columnOrdinal

            Console.WriteLine("Expected to fail:");

            res = board.AssignTask("user999@gmail.com", "board", 1, 0, "user998@gmail.com"); //supposed to be error
            Console.WriteLine("Test 211 " + res);


            Console.WriteLine("Expected to fail:");

            res = board.AssignTask("user999@gmail.com", "board", 2, 0, "user998@gmail.com"); //supposed to be error
            Console.WriteLine("Test 212 " + res);


            Console.WriteLine("Expected to fail:");

            res = board.AssignTask("user999@gmail.com", "board", -1, 0, "user998@gmail.com"); //supposed to be error
            Console.WriteLine("Test 213 " + res);


            //taskId problems
            Console.WriteLine("Expected to fail:");

            res = board.AssignTask("user999@gmail.com", "board", 0, -1, "user998@gmail.com"); //supposed to be error
            Console.WriteLine("Test 214 " + res);


            Console.WriteLine("Expected to fail:");

            res = board.AssignTask("user999@gmail.com", "board", 0, 1, "user998@gmail.com"); //supposed to be error
            Console.WriteLine("Test 215 " + res);


            //assignee problems

            Console.WriteLine("Expected to fail:");

            res = board.AssignTask("user999@gmail.com", "board", 0, 0, null); //supposed to be error
            Console.WriteLine("Test 216 " + res);


            Console.WriteLine("Expected to fail:");

            res = board.AssignTask("user999@gmail.com", "board", 0, 0, ""); //supposed to be error
            Console.WriteLine("Test 217 " + res);


            Console.WriteLine("Expected to fail:");

            res = board.AssignTask("user999@gmail.com", "board", 0, 0, "spikespiegel@gmail.com"); //supposed to be error
            Console.WriteLine("Test 218 " + res);


            Console.WriteLine("Expected to fail:");

            res = board.AssignTask("user999@gmail.com", "board", 0, 0, "user998@gmail.com"); //supposed to be error, user not member
            Console.WriteLine("Test 219 " + res);

            //login problem
            user.logout("user999@gmail.com");

            Console.WriteLine("Expected to fail:");

            res = board.AssignTask("user999@gmail.com", "board", 0, 0, "user998@gmail.com"); //supposed to be error, user not member
            Console.WriteLine("Test 220 " + res);


            //works
            user.login("user999@gmail.com","Password1");
            board.JoinBoard("user998@gmail.com", 12);

            Console.WriteLine("Expected to work:");

            res = board.AssignTask("user999@gmail.com", "board", 0, 0, "user998@gmail.com"); //supposed to be working, user not member
            Console.WriteLine("Test 221 " + res);


            Console.WriteLine("Expected to fail:");

            res = board.AssignTask("user999@gmail.com", "board", 0, 0, "user998@gmail.com"); //supposed to be error, user not assigned
            Console.WriteLine("Test 222 " + res);


            Console.WriteLine("Expected to work?:");

            res = board.AssignTask("user998@gmail.com", "board", 0, 0, "user998@gmail.com"); //supposed to be error, user already assigned
            Console.WriteLine("Test 223 " + res);


            //attempting to moveTask to inProgress and assign
            board.moveTask("user998@gmail.com", "board", 0, 0);

            Console.WriteLine("Expected to fail:");

            res = board.AssignTask("user999@gmail.com", "board", 0, 0, "user998@gmail.com"); //supposed to fail, user not owner
            Console.WriteLine("Test 224 " + res);

            Console.WriteLine("Expected to work:");

            res = board.AssignTask("user998@gmail.com", "board", 1, 0, "user999@gmail.com"); //supposed to be working, user not owner
            Console.WriteLine("Test 225 " + res);


            //attempting to moveTask to done and assign
            board.moveTask("user999@gmail.com", "board", 1, 0);


            Console.WriteLine("Expected to fail:");

            res = board.AssignTask("user998@gmail.com", "board", 1, 0, "user999@gmail.com"); //supposed to fail, user not owner
            Console.WriteLine("Test 226 " + res);


            Console.WriteLine("Expected to fail:");

            res = board.AssignTask("user999@gmail.com", "board", 2, 0, "user998@gmail.com"); //supposed to fail, task in donelist
            Console.WriteLine("Test 227 " + res);

        }

        public void leaveBoardTests()
        {
            string res;
            user.createNewUser("user981@gmail.com", "Tamir12341234");
            user.login("user981@gmail.com", "Tamir12341234");
            board.addBoard("school", "user981@gmail.com"); //14

            user.createNewUser("monkeymode@gmail.com", "Password1");
            user.login("monkeymode@gmail.com", "Password1");
            board.JoinBoard("monkeymode@gmail.com", 14);

            // email problems

            Console.WriteLine("Expected to fail");

            res = board.LeaveBoard("user981@gmail.com", 14); //fail - user is owner, can't leave board
            Console.WriteLine("Test 228 " + res);


            Console.WriteLine("Expected to fail");

            res = board.LeaveBoard("fff", 14); // fail- email isnt valid
            Console.WriteLine("Test 229 " + res);


            Console.WriteLine("Expected to fail");

            res = board.LeaveBoard(null, 14); // fail- email is null
            Console.WriteLine("Test 230 " + res);


            Console.WriteLine("Expected to fail");

            res = board.LeaveBoard("doggo@gmail.com", 14); // fail- email doesnt exists
            Console.WriteLine("Test 231 " + res);

            user.logout("monkeymode@gmail.com");


            Console.WriteLine("Expected to fail");

            res = board.LeaveBoard("monkeymode@gmail.com", 14); //supposed to be error since email is logged out
            Console.WriteLine("Test 232 " + res);

            user.login("monkeymode@gmail.com", "Password1");


            //board id problems

            Console.WriteLine("Expected to fail");

            res = board.LeaveBoard("monkeymode@gmail.com", -14); // fail- board id is negative
            Console.WriteLine("Test 233 " + res);


            Console.WriteLine("Expected to fail");

            res = board.LeaveBoard("monkeymode@gmail.com", 15); // fail- board id doesnt exists
            Console.WriteLine("Test 234 " + res);


            //supposed to work!

            Console.WriteLine("Expected to work");

            res = board.LeaveBoard("monkeymode@gmail.com", 14); // works
            Console.WriteLine("Test 235 " + res);


            Console.WriteLine("Expected to fail");

            res = board.LeaveBoard("monkeymode@gmail.com", 14); // supposed to not work, can't leave from a board that isn't joined
            Console.WriteLine("Test 236 " + res);

            board.JoinBoard("monkeymode@gmail.com", 14);


            Console.WriteLine("Expected to work");

            res = board.LeaveBoard("monkeymode@gmail.com", 14); // works
            Console.WriteLine("Test 237 " + res);

        }


        public void transferOwnershipTests()
        {
            string res;
            user.createNewUser("user982@gmail.com", "Tamir12341234");
            board.addBoard("work", "user982@gmail.com"); //15

            user.createNewUser("user983@gmail.com", "Tamir12341234");
            board.JoinBoard("user983@gmail.com", 15);

            user.createNewUser("user984@gmail.com", "Tamir12341234");


            // email problems

            Console.WriteLine("Expected to fail");

            res = board.TransferOwnership("user982@gmail.com", "user984@gmail.com", "work"); // user isnt joined in board
            Console.WriteLine("Test 238 " + res);


            Console.WriteLine("Expected to work");

            res = board.TransferOwnership("user982@gmail.com","user983@gmail.com", "work"); // works
            Console.WriteLine("Test 239 " + res);


            Console.WriteLine("Expected to fail");

            res = board.TransferOwnership("user982@gmail.com","fff", "work"); // fail- email isnt valid
            Console.WriteLine("Test 240 " + res);


            Console.WriteLine("Expected to fail");

            res = board.TransferOwnership("user982@gmail.com",null, "work"); // fail- email is null
            Console.WriteLine("Test 241 " + res);


            Console.WriteLine("Expected to fail");

            res = board.TransferOwnership("user982@gmail.com","doggo@gmail.com", "work"); // fail- email doesnt exists
            Console.WriteLine("Test 242 " + res);


            Console.WriteLine("Expected to fail");

            res = board.TransferOwnership("user982@gmail.com","user983@gmail.com", "work"); // fail-user is not owner
            Console.WriteLine("Test 243 " + res);


            Console.WriteLine("Expected to fail");

            res = board.TransferOwnership(null, "user982@gmail.com", "work"); // fail-user is not owner
            Console.WriteLine("Test 244 " + res);


            Console.WriteLine("Expected to fail");

            res = board.TransferOwnership("ffff", "user982@gmail.com", "work"); // currentOwner string is illegal 
            Console.WriteLine("Test 245 " + res);


            Console.WriteLine("Expected to fail");

            res = board.TransferOwnership("doggo@gmail.com", "user982@gmail.com", "work"); // currentOwner email doesnt exist
            Console.WriteLine("Test 246 " + res);


            Console.WriteLine("Expected to fail");

            res = board.TransferOwnership("user984@gmail.com", "user982@gmail.com", "work"); // currentOwner not owner and not joined
            Console.WriteLine("Test 247 " + res);


            Console.WriteLine("Expected to fail");

            res = board.TransferOwnership("user983@gmail.com", "user983@gmail.com", "work"); // fail-newOwnerEmail is the same as owner email
            Console.WriteLine("Test 248 " + res);


            //logged out problem

            user.logout("user983@gmail.com");

            Console.WriteLine("Expected to fail");

            res = board.TransferOwnership("user983@gmail.com", "user982@gmail.com", "work"); //supposed to be error since email is logged out
            Console.WriteLine("Test 249 " + res);


            user.login("user983@gmail.com", "Tamir12341234");


            //board name problems

            Console.WriteLine("Expected to fail");

            res = board.TransferOwnership("user983@gmail.com", "user982@gmail.com", null); // fail- board name is null
            Console.WriteLine("Test 250 " + res);


            Console.WriteLine("Expected to fail");

            res = board.TransferOwnership("user983@gmail.com", "user982@gmail.com", "uniiiiiiiiii"); // fail- board name doesnt exists
            Console.WriteLine("Test 251 " + res);


            Console.WriteLine("Expected to work");

            res = board.TransferOwnership("user983@gmail.com", "user982@gmail.com", "work"); // works
            Console.WriteLine("Test 252 " + res);
        }

        public void LoadDataTesting()
        {
            string res;
            board.RemoveData();

            //inserting data into the database
            UserDTO userAdded = new UserDTO("lina@gmail.com", "Password1");
            BoardDTO BoardAdded = new BoardDTO("board", "lina@gmail.com", 0, -1, -1, -1);
            UserBoardDTO userBoard = new UserBoardDTO("lina@gmail.com", 0, "yes");
            TaskDTO taskAdded = new TaskDTO(1, "title", "description", DateTime.Now, new DateTime(2025, 1, 1), 0, 0, "lina@gmail.com");
            TaskIdCounterDTO taskIdCounter = new TaskIdCounterDTO(0, 1);
            BoardIdCounterDTO boardIdCounter = new BoardIdCounterDTO(1);

            userDalController.Insert(userAdded);
            boardDalController.Insert(BoardAdded);
            userBoardDalController.Insert(userBoard);
            taskDalController.Insert(taskAdded);
            taskCounterDalController.Insert(taskIdCounter);
            boardIdCounterDalController.Insert(boardIdCounter);

            board.LoadData();

            Console.WriteLine("All expected to fail:");

            res = user.createNewUser("lina@gmail.com", "Ponies5");
            Console.WriteLine("Test 253 " + res);

            res = user.login("lina@gmail.com", "Password1");
            Console.WriteLine("Test 254 " + res);

            res = board.addBoard("board", "lina@gmail.com");
            Console.WriteLine("Test 255 " + res);

            res = board.LeaveBoard("lina@gmail.com", 0);
            Console.WriteLine("Test 256 " + res);

            res = board.AssignTask("lina@gmail.com", "board", 0, 0, "lina@gmail.com");
            Console.WriteLine("Test 257 " + res);

            Console.WriteLine("All expected to work");

            res = user.logout("lina@gmail.com");
            Console.WriteLine("Test 258 " + res);

            res = user.login("lina@gmail.com", "Password1");
            Console.WriteLine("Test 259 " + res);

            res = board.addTask("lina@gmail.com", "board", "task2", "ponies", new DateTime(2025, 1, 1));
            Console.WriteLine("Test 260 " + res);

            res = board.moveTask("lina@gmail.com", "board", 0, 0);
            Console.WriteLine("Test 261 " + res);
            
            res = board.moveTask("lina@gmail.com", "board", 0, 1);
            Console.WriteLine("Test 262 " + res);

            res = board.editTask("lina@gmail.com","board",1,0,"watermelon",null,null,0);
            Console.WriteLine("Test 263 " + res);

            user.createNewUser("galgal@gmail.com", "Password1");

            res = board.JoinBoard("galgal@gmail.com", 0);
            Console.WriteLine("Test 264 " + res);

            res = board.AssignTask("lina@gmail.com", "board", 1, 0, "galgal@gmail.com");
            Console.WriteLine("Test 265 " + res);
        }

        public void RemoveDataTesting()
        {
            string res;

            //supposed to work
            Console.WriteLine("Expected to work");

            res = board.RemoveData();
            Console.WriteLine("Test 266 " + res);

            user.createNewUser("lina@gmail.com", "Dolphin5");
            board.addBoard("board", "lina@gmail.com");
            board.addTask("lina@gmail.com", "board", "title", "description", new DateTime(2025, 1, 1));

            Console.WriteLine("Expected to work");

            res = board.RemoveData();
            Console.WriteLine("Test 267 " + res);


            Console.WriteLine("All expected to fail:");

            res = user.logout("lina@gmail.com");
            Console.WriteLine("Test 268 " + res);

            res = board.GetBoardName(0);
            Console.WriteLine("Test 269 " + res);

            res = board.addTask("lina@gmail.com", "board", "title", "description", new DateTime(2025, 1, 1));
            Console.WriteLine("Test 270 " + res);

            res = board.moveTask("lina@gmail.com", "board", 0, 0);
            Console.WriteLine("Test 271 " + res);

            res = board.addBoard("board", "lina@gmail.com");
            Console.WriteLine("Test 272 " + res);

            res = board.AssignTask("lina@gmail.com", "board", 0, 0, "lina@gmail.com");
            Console.WriteLine("Test 273 " + res);

            user.createNewUser("galgal@gmail.com", "Password1");
            res = board.JoinBoard("galgal@gmail.com", 0);
            Console.WriteLine("Test 274 " + res);


            Console.WriteLine("All expected to work:");

            res = user.createNewUser("lina@gmail.com", "Dolphin5");
            Console.WriteLine("Test 275 " + res);

            res = board.addBoard("board", "lina@gmail.com");
            Console.WriteLine("Test 276 " + res);

            res = board.addTask("lina@gmail.com", "board", "title", "description", new DateTime(2025, 1, 1));
            Console.WriteLine("Test 277 " + res);

        }
    }
}