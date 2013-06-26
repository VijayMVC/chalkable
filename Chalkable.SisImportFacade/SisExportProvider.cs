using System;
using System.Collections.Generic;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.Data.Common.Enums;
using Chalkable.SisConnector.Services;

namespace Chalkable.SisImportFacade
{
    public class SisExportProvider
    {
        private static Dictionary<ImportSystemTypeEnum, Func<Guid, int, BackgroundTaskService.BackgroundTaskLog, SisExportService>> creators
            = new Dictionary<ImportSystemTypeEnum, Func<Guid, int, BackgroundTaskService.BackgroundTaskLog, SisExportService>>();
        private const string sisExportErrorFmt = "Sis exporter for type {0} does not exists";

        static SisExportProvider()
        {
            //creators.Add(ImportSystemTypeEnum.InfiniteCampus, (schoolId, loggingService, emailService) => new InfiniteCampusConnector.Services.ExportService(schoolId, loggingService, emailService));
        }

        public static SisExportService CreateExportService(ImportSystemTypeEnum systemType, Guid schoolId, int sisSchoolId, BackgroundTaskService.BackgroundTaskLog log)
        {
            if (creators.ContainsKey(systemType))
                return creators[systemType](schoolId, sisSchoolId, log);
            throw new Exception(string.Format(sisExportErrorFmt, systemType));
        }
    }
}