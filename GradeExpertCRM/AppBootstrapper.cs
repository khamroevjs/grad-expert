using GradeExpertCRM.Models;
using GradeExpertCRM.Models.Data;
using GradeExpertCRM.Models.Data.Repositories;
using Splat;

namespace GradeExpertCRM
{
    /// <summary>
    /// For Dependency Injection
    /// </summary>
    public class AppBootstrapper
    {
        public AppBootstrapper()
        {   // Registreted services
            Splat.Locator.CurrentMutable.RegisterConstant(new AppDbContext());
            Splat.Locator.CurrentMutable.RegisterConstant<ISettingsRepository>(new SettingsRepository());
            Splat.Locator.CurrentMutable.RegisterConstant<IDetailsSettingsRepository>(new DetailsSettingsRepository());
            Splat.Locator.CurrentMutable.RegisterConstant<IClientRepository>(new ClientRepository());
            Splat.Locator.CurrentMutable.RegisterConstant<ICarRepository>(new CarRepository());
            Splat.Locator.CurrentMutable.RegisterConstant<IOverallCalculationRepository>(new OverallCalculationRepository());
            Splat.Locator.CurrentMutable.RegisterLazySingleton<ICalculationRepository>(()=>new CalculationRepository());
            Splat.Locator.CurrentMutable.RegisterLazySingleton<IDocumentRepository>(() => new DocumentRepository());
        }
    }
}
