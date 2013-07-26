using System;
using System.Collections.Generic;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.Data.Common.Enums;
using Chalkable.SisConnector.Services;
using Chalkable.StiConnector.Services;

namespace Chalkable.SisImportFacade
{
    public class SisImportProvider
    {
        private static Dictionary<ImportSystemTypeEnum, Func<Guid, int, IList<int>, BackgroundTaskService.BackgroundTaskLog, SisImportService>> creators 
            = new Dictionary<ImportSystemTypeEnum, Func<Guid, int, IList<int>, BackgroundTaskService.BackgroundTaskLog, SisImportService>>();
        private const string sisImportErrorFmt = "Sis importer for type {0} does not exists";
        static SisImportProvider()
        {
            /*creators.Add(ImportSystemTypeEnum.InfiniteCampus, (schoolId, sisSchoolId, sisSchoolYears, loggingService) => 
                new InfiniteCampusConnector.Services.ImportService(schoolId, sisSchoolId, sisSchoolYears, loggingService));*/
            creators.Add(ImportSystemTypeEnum.Sti, (schoolId, sisSchoolId, sisSchoolYears, loggingService) => 
                new ImportService(schoolId, sisSchoolId, sisSchoolYears, loggingService));
        }

        public static SisImportService CreateImportService(ImportSystemTypeEnum systemType, Guid schoolId, int sisSchoolId, IList<int> schoolYearIds, BackgroundTaskService.BackgroundTaskLog log)
        {
            if (creators.ContainsKey(systemType))
                return creators[systemType](schoolId, sisSchoolId, schoolYearIds, log);
            throw new Exception(string.Format(sisImportErrorFmt, systemType));
        }
    }
}
