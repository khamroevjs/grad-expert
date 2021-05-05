using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Permissions;

namespace GradeExpertCRM.Models
{
    public class Calculation
    {
        [Key] public int Id { get; set; }

        public int CarId { get; set; }

        [ForeignKey("CarId")] public Car Car { get; set; }

        public string ComponentName { get; set; }

        public string ComponentImageName { get; set; }

        public bool IsFixPrice { get; set; }

        public double Price { get; set; }

        public int DentQuantity { get; set; }

        public int DentDiameter { get; set; } = 20;

        public double NHours { get; set; }

        public double RemoveDentPrice { get; set; }

        public double DentPrice { get; set; }

        /// <summary>
        /// Material type. Default value: false - "Steel"
        /// </summary>
        public bool IsAluminum { get; set; }

        /// <summary>
        /// Is "Adhesive" applied. Default value: false
        /// </summary>
        public bool IsAdhesive { get; set; }

        public bool IsDismantling { get; set; }

        /// <summary>
        /// Type of repair. Default value: WithoutPainting
        /// </summary>
        public TypeOfRepair TypeOfRepair { get; set; }

        public string TypeOfRepairString { get; set; }

        public double PriceOfPainting { get; set; }

        public List<DismantlingWork> DismantlingWorks { get; set; } = new List<DismantlingWork>();

        public List<SparePart> SpareParts { get; set; } = new List<SparePart>();
    }
}