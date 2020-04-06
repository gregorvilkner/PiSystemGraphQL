using Newtonsoft.Json;
using OSIsoft.AF.Asset;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiGraphQlOwinServer.GraphQlModel
{
    public class GraphQlTsValue
    {
        //[JsonIgnore]
        //public OSIsoft.AF.PI. ThisAfAttribute { get; set; }
        public string value { get; set; }
        public string timeStamp { get; set; }


        public GraphQlTsValue()
        {

        }
    }
}
