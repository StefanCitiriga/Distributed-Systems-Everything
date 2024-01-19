using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DistSysAcwServer.Controllers
{
    public class TalkbackController : BaseController
    {
        /// <summary>
        /// Constructs a TalkBack controller, taking the UserContext through dependency injection
        /// </summary>
        /// <param name="context">DbContext set as a service in Startup.cs and dependency injected</param>
        public TalkbackController(Models.UserContext dbcontext) : base(dbcontext) { }


        #region TASK1
        //    TODO: add api/talkback/hello response
        [HttpGet]
        public async Task<IActionResult> Hello()
        {
            return Ok("Hello World");
        }
        #endregion

        #region TASK1
        //    TODO:
        //       add a parameter to get integers from the URI query
        //       sort the integers into ascending order
        //       send the integers back as the api/talkback/sort response
        //       conform to the error handling requirements in the spec
        [HttpGet]
        public async Task<IActionResult> Sort([FromQuery] int[] integers)
        {
            if (integers == null || integers.Length == 0) return Ok(new int[] { });

            try
            {
                Array.Sort(integers);
                return Ok(integers);
            }
            catch (Exception) 
            {
                return BadRequest();
            }
        }
        #endregion
    }
}
