using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntroSE.Kanban.Backend.DataAccessLayer
{
    public class BoardIdCounterDTO:DTO
    {
        //fields
        public const string counterColumnName = "counter";
        public int counter;
        

        //constructor
        public BoardIdCounterDTO(int Counter) : base(new BoardIdCounterDalController())
        {
            counter = Counter;
        }


        //getters and setters
        public int Counter { get => counter; set { counter = value; _controller.Update(counterColumnName, value); } }

    }
}
