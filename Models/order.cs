using ConsoleApp3.Interfaces;
using LinqToDB.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp3.Models
{
    [Table("orders")]
    public class order : IEntity
    {
        [PrimaryKey, Identity]
        public int id { get; set; }

        [Column("customerid"), NotNull]
        public int customerid { get; set; }

        [Column("productid"), NotNull]
        public int productid { get; set; }

        [Column("quantity"), NotNull]
        public int quantity { get; set; }
    }
}
