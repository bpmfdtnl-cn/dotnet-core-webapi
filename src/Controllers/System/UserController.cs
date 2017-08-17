/**
 * Created by w on 2017/7/6.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using W.WebApi.Helper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Infrastructure;
using W.WebApi.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace W.WebApi.Controllers
{
    [Route("api/[controller]")]
    public class UserController : BaseController
    {
        public UserController(ApiContext context) : base(context)
        {
        }

        // GET: api/values
        //[HttpGet]
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2", "UserController" };
        //}

        [HttpGet]
        public Result Get()
        {
            return Result.SAVE_SUCCESS;
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public Result Post([FromBody]string value)
        {
            string requestBody = new System.IO.StreamReader(Request.Body).ReadToEnd();

            return Result.SAVE_SUCCESS;
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
