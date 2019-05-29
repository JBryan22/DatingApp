using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Controllers
{
    //http://localhost:5000/api/values
    //[controller] is filled by the name of the controller (in this case Values)
    [Route("api/[controller]")]
    //ApiController enforces routing and enables authorization
    [ApiController]
    //using ControllerBase instead of controller because this is an API. Controller can return Views, whereas ControllerBase cannot.
    //we are not returning views because this is just an API. Our Angular project is what is returning views.
    public class ValuesController : ControllerBase
    {
        private readonly DataContext _db;
        public ValuesController(DataContext db)
        {
            _db = db;
        }
        // GET api/values
        [HttpGet]
        public async Task<IActionResult> GetValues()
        {
            var values = await _db.Values.ToListAsync();
            return Ok(values);
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public async Task<ActionResult<string>> GetValue(int id)
        {
            var value = await _db.Values.FirstOrDefaultAsync(v => v.Id == id);
            return Ok(value);
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
