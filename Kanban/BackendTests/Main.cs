using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntroSE.Kanban.Backend;
using IntroSE.Kanban.Backend.BusinessLayer;
using IntroSE.Kanban.Backend.ServiceLayer;
using IntroSE.Kanban.Backend.DataAccessLayer;
using System.Text.Json;

namespace BackendTests
{
    class main
    {
        static void Main(String[] args)
        {
            UserManagement um = new UserManagement();
            UserService us = new UserService(um);
            BoardService bs = new BoardService(um);


            UserTesting ut = new UserTesting(us);
            BoardTesting bt = new BoardTesting(us, bs);

            bs.RemoveData();
            ut.runTests();
            bt.runTests();
            bs.RemoveData();
        }

    } 
}



            




           








     

          


        
        
