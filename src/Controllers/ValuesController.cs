using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Infrastructure;
using W.WebApi.Helper;
using W.WebApi.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace W.WebApi.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : BaseController
    {
        public ValuesController(ApiContext context) : base(context)
        {
        }
        // GET: api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2", "ValuesController" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public Result Get(int id)
        {
            DataTableCore table = new DataTableCore();
            table.Columns.Add(new DataColumnCore()
            {
                ColumnType = typeof(string)
                ,
                ColumnName = "Id"
            });
            table.Columns.Add(new DataColumnCore()
            {
                ColumnType = typeof(string)
                ,
                ColumnName = "Name"
            });
            table.Columns.Add(new DataColumnCore()
            {
                ColumnType = typeof(DateTime)
                ,
                ColumnName = "UpdaterTime"
            });
            table.Columns.Add(new DataColumnCore()
            {
                ColumnType = typeof(int)
                ,
                ColumnName = "Delete"
            });
            table.Rows.Add(new DataRowCore(table.Columns, new object[] { Guid.NewGuid().ToString("N"), "Name1", DateTime.Now, 0 }));
            table.Rows.Add(new DataRowCore(table.Columns, new object[] { Guid.NewGuid().ToString("N"), "Name2", DateTime.Now, 1 }));

            var users = _context.Users;

            Result result = new Result()
            {
                success = true,
                count=table.Rows.Count,
                data=table.ToArray()
            };

            return result;
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
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
