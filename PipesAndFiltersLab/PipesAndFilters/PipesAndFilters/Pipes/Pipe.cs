﻿using PipesAndFilters.Filters;
using PipesAndFilters.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PipesAndFilters.Pipes
{
    class Pipe : IPipe
    {
        private List<IFilter> Filters;
        
        public Pipe() 
        {
            Filters= new List<IFilter>();   
        }   
        public void RegisterFilter(IFilter filter)
        {
            Filters.Add(filter);
        }

        public IMessage ProcessMessage(IMessage message)
        {
            foreach (IFilter filter in Filters) 
            {
                message = filter.Run(message);
            }
            return message; 
        }
    }
}
