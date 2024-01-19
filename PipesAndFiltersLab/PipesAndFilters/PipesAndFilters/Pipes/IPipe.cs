using PipesAndFilters.Filters;
using PipesAndFilters.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PipesAndFilters.Pipes
{
    interface IPipe
    {
        public void RegisterFilter(IFilter filter);
        public IMessage ProcessMessage(IMessage message);
    }
}
