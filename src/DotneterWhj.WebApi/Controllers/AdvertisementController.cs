using DotneterWhj.IServices;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DotneterWhj.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdvertisementController : ControllerBase
    {
        private readonly IAdvertisementService _advertisementService;

        public AdvertisementController(IAdvertisementService advertisementService)
        {
            this._advertisementService = advertisementService;
        }

        // GET: api/<AdvertisementController>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(await _advertisementService.QueryAsync());
        }

        // GET api/<AdvertisementController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<AdvertisementController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<AdvertisementController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<AdvertisementController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
