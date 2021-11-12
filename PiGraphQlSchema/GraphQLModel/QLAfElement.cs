using System.Collections.Generic;

namespace PiGraphQlSchema.GraphQlModel
{
    public class QLAfElement
    {
        public string name { get; set; }
        public string template { get; set; }
        public string path { get; set; }

        public List<QLAfElement> afElements { get; set; }
        public List<QLAfAttribute> afAttributes { get; set; }


    }
}
