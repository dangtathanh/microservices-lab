using Microsoft.AspNetCore.Mvc;

namespace GRPCLab.ContactService.Controller.V2
{
    [ApiVersion("2")]
    public class ContactsController : BaseController<ContactsController>
    {
        public ContactsController(ILoggerFactory loggerFactory)
            : base(loggerFactory)
        {

        }

        [HttpGet]
        [Route("")]
        public IActionResult GetList()
        {
            var result = new List<object>
            {
                new { Id = 1, Name = "Jacki" },
                new { Id = 2, Name = "Kikoi" },
                new { Id = 3, Name = "Nemoi" }
            };
            return Ok(result);
        }

        [HttpGet]
        [Route("{id}")]
        public IActionResult GetItem(int id)
        {
            var result = new { Id = 1, Name = "Jacki" };
            return Ok(result);
        }

        [HttpPost]
        [Route("{id}")]
        public IActionResult AddItem(int id, [FromBody] object data)
        {
            return Ok(data);
        }

        [HttpPut]
        [Route("{id}")]
        public IActionResult EditItem(int id, [FromBody] object data)
        {
            return Ok(data);
        }

        [HttpDelete]
        [Route("{id}")]
        public IActionResult DeleteItem(int id)
        {
            return NoContent();
        }
    }
}
