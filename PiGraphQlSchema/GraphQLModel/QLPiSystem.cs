﻿using System.Collections.Generic;

namespace PiGraphQlSchema.GraphQlModel
{
    public class QLPiSystem
    {
        public string name { get; set; }

        public List<QLAfDatabase> afDbs { get; set; }

    }
}
