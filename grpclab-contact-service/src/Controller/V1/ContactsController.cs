using GRPCLab.ContactService.Models.V1;
using GRPCLab.ContactService.Servives;
using GRPCLab.ContactService.Servives.V1;
using Microsoft.AspNetCore.Mvc;

namespace GRPCLab.ContactService.Controller.V1
{
    [ApiVersion("1")]
    public class ContactsController : BaseController<ContactsController>
    {
        private readonly IProfileService<ProfileService> _profileService;
        public ContactsController(IProfileService<ProfileService> profileService,
                                    ILoggerFactory loggerFactory)
            : base(loggerFactory)
        {
            _profileService = profileService;
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> GetList()
        {
            var result = await _profileService.GetManyAsync(new GetProfilesRequest
            {
                Ids = new List<int> { 1, 2, 3 }
            });
            return Ok((result as GetProfilesResult).Result?.Results);
        }

        [HttpGet]
        [Route("{id}")]
        public IActionResult GetItem(int id)
        {
            var result = new { Id = 1, Name = "Jack" };
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
