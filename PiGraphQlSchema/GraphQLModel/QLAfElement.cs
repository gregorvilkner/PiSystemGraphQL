using System.Collections.Generic;

namespace PiGraphQlSchema.GraphQlModel
{
    public class QLAfElement
    {
        public string name { get; set; }
        public string template { get; set; }
        public string path { get; set; }

        public List<QLAfElement> qlAfElements { get; set; }
        public List<QLAfAttribute> qlAfAttributes { get; set; }


    }
}
