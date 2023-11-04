﻿using System.Collections.Generic;

namespace Bee.SQLite.Models
{
    public class Insert
    {
        public bool execute { get; set; }
        public string message { get; set; }
        public bool dublicate { get; set; }
        public List<int?> insertedIds { get; set; }

        public Insert() 
        {
            execute = false;
            message = null;
            dublicate = false;
            insertedIds = new List<int?>();
        }
    }
}
