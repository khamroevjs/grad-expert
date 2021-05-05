using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing.Printing;
using System.Text;

namespace GradeExpertCRM.Models
{
    /// <summary>
    /// DTO.
    /// </summary>
    public class Document
    {
        [Key]
        public int Id { get; set; }

        public int CarId { get; set; }

        [ForeignKey("CarId")]
        public Car Car { get; set; }

        public string Title { get; set; }

        public DateTime CreationDate { get; set; }

        public byte[] Content { get; set; }
    }
}
