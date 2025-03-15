using ConsoleApp3.Interfaces;
using LinqToDB.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp3.Models
{
    [Table("products")]
    public class product : IEntity
    {
        [PrimaryKey, Identity]
        public int id { get; set; }

        [Column("name"), NotNull]
        public string name { get; set; }

        [Column("description")]
        public string description { get; set; }

        [Column("stockquantity"), NotNull]
        public int stockquantity { get; set; }

        [Column("price"), NotNull]
        public decimal price { get; set; }
    }
}
