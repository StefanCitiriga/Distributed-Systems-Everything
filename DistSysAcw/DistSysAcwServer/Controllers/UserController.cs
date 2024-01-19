using DistSysAcwServer.DataAccess;
using Microsoft.AspNetCore.Authorization;
using DistSysAcwServer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using System.Security.Cryptography;
using static System.Net.Mime.MediaTypeNames;
using Newtonsoft.Json;

namespace DistSysAcwServer.Controllers
{
    [ApiController]
    public class UserController : BaseController
    {
        readonly DatabaseCRUD _databaseCRUD;
        public UserController(DatabaseCRUD databaseCRUD, Models.UserContext dbcontext) : base(dbcontext)
        {
            _databaseCRUD = databaseCRUD;
        }

        [HttpGet]
        public IActionResult Test()
        {
            return Ok("it got to this point so it means its the auth for some reason.");
        }

        // GET: api/user/new?username=UserOne
        //[HttpGet("[Action]=New")]
        [HttpGet("New")]
        public async Task<IActionResult> CheckUser([FromQuery] string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return Ok("False - User Does Not Exist! Did you mean to do a POST to create a new user?");
            }

            bool userExists = await _databaseCRUD.UserExists(username);
            if (userExists)
            {
                return Ok("True - User Does Exist! Did you mean to do a POST to create a new user?");
            }
            else
            {
                return Ok("False - User Does Not Exist! Did you mean to do a POST to create a new user?");
            }
        }

        //api/user/new
        
        [HttpPost]
        public async Task<IActionResult> New([FromBody] string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return BadRequest("Oops. Make sure your body contains a string with your username and your Content-Type is Content-Type:application/json");
            }

            bool userExists = await _databaseCRUD.UserExists(username);
            if (userExists)
            {
                return StatusCode(403, "Oops. This username is already in use. Please try again with a new username.");
            }
            else
            {
                string apiKey = await _databaseCRUD.CreateNew(username);
                return Ok(apiKey);
            }
        }    
        
        [HttpDelete]
        public async Task<IActionResult> RemoveUser([FromQuery] string username)
        {
            var apikey = Request.Headers["ApiKey"].ToString();

            if (await _databaseCRUD.Check(apikey, username))
            {
                if (await _databaseCRUD.Delete(apikey))
                {
                    return Ok(true);
                }
                else return Ok(false);
            }
            else return Ok("Username and ApiKey not from the same user");
        }
        [Authorize(Roles ="Admin")]
        [HttpPost]
        public async Task<IActionResult> ChangeRole([FromBody] UsernameAndRole json)
        {
            if(string.IsNullOrEmpty(json.Username) || string.IsNullOrEmpty(json.Role)) 
            {
                return BadRequest("NOT DONE: An error occured");
            }

            if(! await _databaseCRUD.UserExists(json.Username))
            {
                return BadRequest("NOT DONE: Username does not exist");
            }

            if(json.Role != "User" && json.Role != "Admin")
            {
                return BadRequest("NOT DONE: Role does not exist");
            }

            var result = await _databaseCRUD.UpdateRole(json.Username, json.Role);
            if(result) return Ok("DONE");

            return BadRequest("NOT DONE: An error occured");
        }

        public class UsernameAndRole
        {
            public string Username { get; set; }
            public string Role { get; set; }
        }
    }
}
