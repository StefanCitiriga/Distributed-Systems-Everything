﻿using System;
using System.Collections.Generic;
using System.Text;

namespace PipesAndFilters
{
    class Message : IMessage
    {
        public Dictionary<string, string> Headers { get; set; }
        public string Body { get; set; }

        public Message()
        {
            Headers = new Dictionary<string, string>();
        }
    }
}
