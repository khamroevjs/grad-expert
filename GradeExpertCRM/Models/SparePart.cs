using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GradeExpertCRM.Models
{
    public class SparePart
    {
        [Key]
        public int Id { get; set; }

        public int CalculationId { get; set; }

        [ForeignKey(nameof(CalculationId))]
        public Calculation Calculation { get; set; }

        public bool IsApplied { get; set; } //TODO May be redundant 

        public int Code { get; set; }

        public string Name { get; set; }

        public double Price { get; set; }

        public int Quantity { get; set; }
    }
}