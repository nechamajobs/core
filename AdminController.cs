using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using OurApi.Models;
using OurApi.Services;

using OurApi.Interfaces;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace OurApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class Admincontroller : ControllerBase
    {
        private Iuser userService;
        private Iglass glassesService;
        public Admincontroller(Iuser users,Iglass glassesService)
        {
            this.userService = users;
            this.glassesService=glassesService;
        }

        [HttpGet]
       [Authorize(Policy = "Admin")]
        public ActionResult<List<User>> GetAll() =>
            userService.GetAll();

        [HttpGet("{id}")]
        [Authorize(Policy ="User")]

        public ActionResult<User> Get(int id)
        {
            if ((int.Parse(User.FindFirst("id")?.Value!) != id) && User.FindFirst("type")?.Value != "Admin")
                return Unauthorized();
            var user = userService.Get(id);
            if (user == null)
                return NotFound();
            return user;
        }

//   public ActionResult Post(User newUser)
//     {
//         var newId = UsersService.Add(newUser);

//         return CreatedAtAction("Post",
//             new { id = newId }, UsersService.GetById(newId));
//     }
      [HttpPost]
      [Authorize(Policy = "Admin")]
        public ActionResult Post(User U)
        {
         Console.WriteLine("-----------------------------------");
            Console.WriteLine(U);

             var newId = userService.Add(U);

        return CreatedAtAction("Post",
            new { id = newId}, userService.Get(newId));

        
        //    var newId = userService.Add(U);
        //      return CreatedAtAction("Post",
        //     new { id = newId }, userService.Get(newId));
        //     // return CreatedAtAction(nameof(Create), new { id = U.id }, U);

        }
        [HttpPost]
        [Route("/login")]
        public ActionResult<objectToReturn> Login([FromBody] User User)
        {

            int UserExistID = userService.ExistUser(User.Username, User.Password);
            Console.WriteLine("-----------------------------------");
                        Console.WriteLine(UserExistID);

            // var dt = DateTime.Now;
            if (UserExistID == -1)
            {
                return Unauthorized();
            }

            var claims = new List<Claim> { };
            if (User.Password == "123")
                claims.Add(new Claim("type", "Admin"));
            else
                claims.Add(new Claim("type", "User"));

            claims.Add(new Claim("id", UserExistID.ToString()));

            var token = GlassesTokenService.GetToken(claims);
            Console.WriteLine(token);
            return new OkObjectResult(new { Id = UserExistID, token = GlassesTokenService.WriteToken(token) });
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "User")]
        public IActionResult Update(int id, User u)
        {
            if (id != u.id)
                return BadRequest();

            var existinguser = userService.Get(id);
            if (existinguser == null)
                return NotFound();

            userService.Update(u);

            return NoContent();
        }
          [HttpDelete("{id}")]
        [Authorize(Policy = "User")]

        public IActionResult Delete(int id)
        {
            var gla = userService.Get(id);
            if (gla is null)
                return NotFound();

            userService.Delete(id);
            glassesService.DeleteAll(id);

            return Content(userService.Count.ToString());
        }
    }



}

public class objectToReturn
{
    public int Id { get; set; }

    public string token { get; set; }
}