using DistSysAcwServer.DataAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DistSysAcwServer.Controllers
{
    [ApiController]
    [Authorize(Roles= "Admin, User")]
    public class ProtectedController : BaseController
    {
        readonly DatabaseCRUD _databaseCRUD;
        public ProtectedController(DatabaseCRUD databaseCRUD, Models.UserContext dbcontext) : base(dbcontext)
        {
            _databaseCRUD = databaseCRUD;
        }

        [HttpGet]
        public async Task<IActionResult> Hello()
        {
            string apikey = Request.Headers["ApiKey"].FirstOrDefault();
            if(string.IsNullOrEmpty(apikey))
            {
                return BadRequest("ApiKey not found in header.");
            }
            var user = await _databaseCRUD.GetUserObject(apikey);
            if(user == null)
            {
                return Unauthorized("Invalid ApiKey");
            }
            return Ok("Hello " + user.UserName);
        }
    }
}
