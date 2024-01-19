using PipesAndFilters.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PipesAndFilters.Filters
{
    class TimestampFilter: IFilter
    {
        public IMessage Run(IMessage message)
        {
            message.Headers.Add("Timestamp", DateTime.Now.ToString());
            return message; 
        }
    }
}
