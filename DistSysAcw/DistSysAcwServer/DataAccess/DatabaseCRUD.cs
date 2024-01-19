using DistSysAcwServer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DistSysAcwServer.DataAccess
{
    public class DatabaseCRUD
    {
        public async Task<string> CreateNew(string Username)
        {
            using (var ctx = new UserContext())
            {
                string apikey = Guid.NewGuid().ToString();
                bool isFirstUser = !ctx.Users.Any();
                User user = new User()
                {
                    UserName = Username,
                    Role = isFirstUser ? "Admin": "User",
                    ApiKey = apikey
                };
                ctx.Users.Add(user);
                await ctx.SaveChangesAsync();
                return apikey;
            }
        }

        public async Task<bool> UserExists(string username)
        {
            using (var ctx = new UserContext())
            {
                var user = await ctx.Users.FirstOrDefaultAsync(x => x.UserName == username);
                return user != null;
            }
        }

        public async Task<bool> Check(string apikey)
        {
            using (var ctx = new UserContext()) 
            {
                var user = await ctx.Users.FirstOrDefaultAsync(x => x.ApiKey == apikey);
                return user != null;
            }
        }

        public async Task<bool> Check(string apikey, string username) 
        {
            using (var ctx = new UserContext()) 
            {
                var user =await ctx.Users.FirstOrDefaultAsync(x => x.ApiKey == apikey && x.UserName == username);
                return user != null;
            }
        }

        public async Task<User> GetUserObject(string apikey)
        {
            using (var ctx = new UserContext())
            {
                var user = await ctx.Users.FirstOrDefaultAsync(x => x.ApiKey == apikey);
                return user;
            }
        }

        public async Task<bool> Delete(string apikey) 
        {
            using (var ctx = new UserContext())
            {
                var user = await ctx.Users.FirstOrDefaultAsync(x => x.ApiKey == apikey);
                if (user != null)
                {
                    ctx.Users.Remove(user);
                    ctx.SaveChanges();
                    return true;
                }
                else return false;
            }
        }

        public async Task<bool> UpdateRole(string username, string role)
        {
            using(var ctx = new UserContext())
            {
                var user = await ctx.Users.FirstOrDefaultAsync(x => x.UserName == username);
                if (user == null) { return false; }
                user.Role = role;
                await ctx.SaveChangesAsync();
                return true;
            }
        }
    }
}
