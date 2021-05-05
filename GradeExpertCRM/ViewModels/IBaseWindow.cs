using System.Collections.ObjectModel;
using GradeExpertCRM.Models;

namespace GradeExpertCRM.ViewModels
{
    public interface IBaseWindow
    {
        /// <summary>
        /// Implements getting access to the reference to 
        /// the changing content of the app window.
        /// </summary>
        public ViewModelBase Content { get; set; }

        public ILanguageProvider Localization { get; }

        public string Language { set; }

        public bool IsAdmin { get; set; }
    }
}