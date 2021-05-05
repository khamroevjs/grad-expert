using System.ComponentModel.DataAnnotations;

namespace GradeExpertCRM.Models
{
    public class Settings
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public double RemoveDentsPrice { get; set; }
        [Required]
        public double DismantlingPrice { get; set; }
        [Required]
        public double FinalProcessing { get; set; }
        [Required]
        public double AntiCorrosion { get; set; }
        [Required]
        public double PreparingTool { get; set; }
        [Required]
        public double FinalProcessingMax { get; set; }
        [Required]
        public double TechnicalWash { get; set; }
        [Required]
        public double SalonCleaning { get; set; }
        [Required]
        [Range(0, 100)]
        public int AluminumPercent { get; set; }
        [Required]
        [Range(0, 100)]
        public int GlueTechniquePercent { get; set; }
        [Required]
        [Range(0, 100)]
        public int UnderPantingPercent { get; set; }
        [Required]
        [Range(0, 100)]
        public int TaxPercent { get; set; }
        public string Language { get; set; } = "Russian";
    }
}
