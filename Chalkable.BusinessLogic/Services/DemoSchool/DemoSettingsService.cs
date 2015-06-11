﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool
{


    public class DemoSettingsStorage:BaseDemoIntStorage<DistrictSettings>
    {
        public DemoSettingsStorage()
            : base(null, true)
        {
        }
    }

    public class DemoSettingsService : DemoSchoolServiceBase, ISettingsService
    {
        private DemoSettingsStorage SettingsStorage { get; set; }
        public DemoSettingsService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
            SettingsStorage = new DemoSettingsStorage();
        }

        public void AddSettings(IList<DistrictSettings> settings)
        {
            SettingsStorage.Add(settings);
        }

        public void Edit(IList<DistrictSettings> settings)
        {
            throw new NotImplementedException();
        }

        public DistrictSettings GetSetting(string category, string setting)
        {
            return SettingsStorage.GetAll().First(x => x.Category == category && x.Setting == setting);
        }

        public void Delete(IList<DistrictSettings> settings)
        {
            throw new NotImplementedException();
        }
    }

}
