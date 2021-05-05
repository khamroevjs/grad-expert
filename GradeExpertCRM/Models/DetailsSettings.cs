using System.ComponentModel.DataAnnotations;
using Avalonia.Media.Imaging;

namespace GradeExpertCRM.Models
{
    public class DetailsSettings
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }

        public byte[] Image { get; set; }

        [Required]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Index must be number")]
        public string Index { get; set; }

        [Required]
        public string Area { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        [RegularExpression(@"^[0-9 ()+-]+$", ErrorMessage = "Incorrect format of number")]
        public string PhoneNumber { get; set; }

        [Required]
        [RegularExpression(@"^[\w-\.]+@([\w-]+\.)+[\w-]{1,}$", ErrorMessage = "Incorrect format of email")]
        public string Email { get; set; }

        /// <summary>
        /// ИНН - TIN (Taxpayer Identification Number)
        /// </summary>
        [Required]
        [RegularExpression(@"^\d{10,}$", ErrorMessage = "Must be 10 digits")]
        public string TIN { get; set; }

        /// <summary>
        /// КПП
        /// </summary>
        [Required]
        [RegularExpression(@"^\d{9,}$", ErrorMessage = "Must be 9 digits")]
        public string CRR { get; set; }

        [Required]
        public string Bank { get; set; }

        /// <summary>
        /// БИК - BIC (Bank Indentification Code)
        /// </summary>
        [Required]
        [RegularExpression(@"^\d{9,}$", ErrorMessage = "Must be 9 digits")]
        public string BIC { get; set; }

        /// <summary>
        /// Расчетный счет
        /// </summary>
        [Required]
        [RegularExpression(@"^\d{20,}$", ErrorMessage = "Must be 20 digits")]
        public string PaymentAccount { get; set; }

        /// <summary>
        /// Корреспондирующий счёт — Corresponding account
        /// </summary>
        [Required]
        [RegularExpression(@"^\d{20,}$", ErrorMessage = "Must be 20 digits")]
        public string CorrespondentAccount { get; set; }
    }
}
