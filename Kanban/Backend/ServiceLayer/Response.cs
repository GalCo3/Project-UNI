using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using IntroSE.Kanban.Backend.BusinessLayer;
using System.Text.Json.Serialization;

namespace IntroSE.Kanban.Backend.ServiceLayer
#nullable enable
{
    public class Response<T>
    {
        [JsonInclude]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public readonly string? ErrorMessage;


        [JsonInclude]
        [JsonIgnore(Condition =JsonIgnoreCondition.WhenWritingNull)]
        public readonly T? ReturnValue;

        
        [JsonConstructor]
        public Response (String? errorMessage, T? returnValue)
        {
            ErrorMessage = errorMessage;
            ReturnValue = returnValue;
        }
        
    }
}
