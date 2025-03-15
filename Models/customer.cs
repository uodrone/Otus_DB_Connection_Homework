using ConsoleApp3.Interfaces;
using LinqToDB.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp3.Models
{
    [Table("customers")]
    public class customer : IEntity
    {
        [PrimaryKey, Identity]
        public int id { get; set; }

        [Column("firstname"), NotNull]
        public string firstname { get; set; }

        [Column("lastname"), NotNull]
        public string lastname { get; set; }

        [Column("age")]
        public int age { get; set; }
    }
}
