using PipesAndFilters.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PipesAndFilters.Filters
{
    class AuthenticateFilter: IFilter
    {
        public IMessage Run(IMessage message)
        {
            if (message.Headers.ContainsKey("User")) 
                ServerEnvironment.SetCurrentUser(int.Parse(message.Headers["User"]));
            return message;
        }
    }
}
