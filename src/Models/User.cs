using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace W.WebApi.Models
{
    public class User
    {
        [Key,StringLength(32)]
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        [Column("delete")]
        public int Delete { get; set; }
    }
}
