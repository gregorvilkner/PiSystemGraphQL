using System.Collections.Generic;

namespace PiGraphQlSchema.GraphQlModel
{
    public class QLAfDatabase
    {
        public string name { get; set; }

        public string path { get; set; }

        public List<QLAfElement> qlAfElements { get; set; }



    }
}
