using BIT.SOLUTION.Common;
using System;
using System.ComponentModel.DataAnnotations;


namespace BIT.SOLUTION.MODEL
{
    [TableName("AspNetUsers")]
    public class Acount: BaseEntity
    {
        [Key]
        [Column]
        public Guid Id { set; get; }
        [Column]
        public string Username { set; get; }
        [NotMapped]
        public string ListApp { set; get; }
    }
}
