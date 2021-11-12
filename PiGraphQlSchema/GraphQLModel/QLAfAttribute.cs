using System.Collections.Generic;

namespace PiGraphQlSchema.GraphQlModel
{
    public class QLAfAttribute
    {
        public string name { get; set; }
        public string value { get; set; }
        public string timeStamp { get; set; }
        public string uom { get; set; }

        public List<QLAfAttribute> afAttributes { get; set; }
        
        public List<QLTsValue> tsPlotValues { get; set; }

    }
}
