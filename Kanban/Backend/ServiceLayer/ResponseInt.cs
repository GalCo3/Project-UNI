using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace IntroSE.Kanban.Backend.ServiceLayer
{
    public class ResponseInt
    {
        

        [JsonInclude]
        //[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public readonly int ReturnValue;


        [JsonConstructor]
        public ResponseInt(int returnValue)
        {
            
            ReturnValue = returnValue;
        }

    }
}
