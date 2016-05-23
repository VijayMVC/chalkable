using System.Collections.Generic;

namespace Chalkable.BusinessLogic.Services.School
{

    public interface IPanoramaSettingsService
    {
        void Save<TSettings>(TSettings settings);
        TSettings Get<TSettings>();
        TSettings Restore<TSettings>();
        void SaveDefault<TDefaultSettings>();
        TDefaultSettings GetDefaultSettings<TDefaultSettings>();
        
    }

    public class PanoramaSettingsService : SchoolServiceBase, IPanoramaSettingsService
    {
        public PanoramaSettingsService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }

        public void Save<TSettings>(TSettings settings)
        {
            throw new System.NotImplementedException();
        }
        public TSettings Get<TSettings>()
        {
            return default(TSettings);
            throw new System.NotImplementedException();
        }
        public TSettings Restore<TSettings>()
        {
            return default(TSettings);
            throw new System.NotImplementedException();
        }
        public void SaveDefault<TDefaultSettings>()
        {
            throw new System.NotImplementedException();
        }
        public TDefaultSettings GetDefaultSettings<TDefaultSettings>()
        {
            throw new System.NotImplementedException();
        }
    }


    public class ClassProfilePanoramaSettings
    {
        public IList<int> SchoolYearIds { get; set; } 
        public IList<StandardizedTestFilter> StandardizedTestFilters { get; set; }
    }

    public class PersonProfilePanoramaSettings
    {
        public int CourseTypeId { get; set; }
        public IList<int> SchoolYearIds { get; set; }
        public IList<StandardizedTestFilter> StandardizedTestFilters { get; set; }
    }

    public class StandardizedTestFilter
    {
        public int StandardizedTestId { get; set; }
        public int ComponentId { get; set; }
        public int ScoreTypeId { get; set; }
    }
}
