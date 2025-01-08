using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace IntroSE.Kanban.Backend.ServiceLayer
{
    public static class JsonController
    {
        static JsonSerializerOptions js = new() { WriteIndented = true };
        public static string Serialize<T>(T o)
        {
            return JsonSerializer.Serialize(o, js);
        }

        public static T DeSerialize<T>(string os)
        {

            return JsonSerializer.Deserialize<T>(os, js);

        }
    }
}
