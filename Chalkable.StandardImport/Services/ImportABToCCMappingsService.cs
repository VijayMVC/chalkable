using System;
using System.Collections.Generic;
using Chalkable.Common;
using Chalkable.Data.Master.Model;
using Chalkable.StandardImport.Models;

namespace Chalkable.StandardImport.Services
{

    public class ImportABToCCMappingsService: ImportService
    {
        public ImportABToCCMappingsService(string connectionString) : base(connectionString)
        {
        }

        public override void Import(byte[] data)
        {
            CsvContainer = new ABToCCMappingImportModel(data);
            var standards = ServiceLocatorMaster.CommonCoreStandardService.GetStandards();
            ISet<Guid> stIds = new HashSet<Guid>();
            foreach (var standard in standards) stIds.Add(standard.Id);


            var mappings = ServiceLocatorMaster.CommonCoreStandardService.GetABToCCMappings(null, null);
            ISet<Pair<Guid,Guid>> addedMappings = new HashSet<Pair<Guid, Guid>>();
            foreach (var mapping in mappings)
            {
                addedMappings.Add(new Pair<Guid, Guid>(mapping.AcademicBenchmarkId, mapping.CCStandardRef));
            }

            var res = new List<ABToCCMapping>();
            for (int i = 0; i < CsvContainer.Rows.Count; i++)
            {
                if (!CsvContainer.ValideteCellHasValue(i, ABToCCMappingImportModel.AB_ID_COLUMN, "no academic benchmark id ")) continue;
                if (!CsvContainer.ValideteCellHasValue(i, ABToCCMappingImportModel.CC_ID_COLUMN, "no common core id ")) continue;

                Guid abId, ccId;
                if (!Guid.TryParse(CsvContainer.GetValue(i, ABToCCMappingImportModel.AB_ID_COLUMN), out abId))
                {
                    CsvContainer.Rows[i].State = CsvRowState.Failed;
                    CsvContainer.Rows[i].ErrorMessage = "Invalid academic benchmark id";
                    continue;
                }
                if (!Guid.TryParse(CsvContainer.GetValue(i, ABToCCMappingImportModel.CC_ID_COLUMN), out ccId))
                {
                    CsvContainer.Rows[i].State = CsvRowState.Failed;
                    CsvContainer.Rows[i].ErrorMessage = "Invalid common core standard id";
                    continue;
                }
                var pair = new Pair<Guid, Guid>(abId, ccId);
                if(addedMappings.Contains(pair)) continue;
                if (!stIds.Contains(ccId))
                {
                    CsvContainer.Rows[i].State = CsvRowState.Failed;
                    CsvContainer.Rows[i].ErrorMessage = string.Format("There is no common core standard with such id  = '{0}'", ccId);
                    continue;
                }
                res.Add(new ABToCCMapping {AcademicBenchmarkId = abId, CCStandardRef = ccId});
                addedMappings.Add(pair);
            }
            ServiceLocatorMaster.CommonCoreStandardService.AddABToCCMapping(res);
        }
    }
}
