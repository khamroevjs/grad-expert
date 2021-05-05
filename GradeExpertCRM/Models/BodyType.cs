using System.ComponentModel;

namespace GradeExpertCRM.Models
{
    public enum BodyType
    {
        [Description("КОМПАКТ")]
        Compact,
        [Description("КУПЕ")]
        Coupe,
        [Description("КАБРИОЛЕТ")]
        Cabrio,
        [Description("СЕДАН")]
        Sedan,
        [Description("ХЕТЧБЭК")]
        Hatchback,
        [Description("УНИВЕРСАЛ")]
        Universal,
        [Description("ЛИФТБЕК")]
        Liftback,
        [Description("МИНИВЕН")]
        Minivan,
        [Description("КРОССОВЕР")]
        Crossover,
        [Description("МИКРОАВТОБУС")]
        Minibus,
        [Description("ВНЕДОРОЖНИК")]
        SUV,
        [Description("ФУРГОН")]
        Van,
        [Description("ПИКАП")]
        Pickup
    }
}
