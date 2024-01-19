using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace DistSysAcwServer.Models
{
    public class User
    {
        #region Task2
        // TODO: Create a User Class for use with Entity Framework
        // Note that you can use the [key] attribute to set your ApiKey Guid as the primary key
        [Key] public string ApiKey { get; set; }
        public User() { }
        public string UserName { get; set; }
        public string Role { get; set; }
        #endregion
    }

    #region Task13?
    // TODO: You may find it useful to add code here for Logging
    #endregion


}