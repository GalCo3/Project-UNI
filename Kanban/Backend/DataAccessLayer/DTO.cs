using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntroSE.Kanban.Backend.DataAccessLayer
{
    public abstract class DTO
    {
        //fields
        protected DalController _controller;


        //constructor
        protected DTO(DalController controller)
        {
            _controller = controller;
        }

    }
}
