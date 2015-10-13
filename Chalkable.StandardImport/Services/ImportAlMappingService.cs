using System;
using System.Collections.Generic;
using Chalkable.Common;
using Chalkable.Data.Master.Model;
using Chalkable.StandardImport.Models;

namespace Chalkable.StandardImport.Services
{

    public class NSSImportService : ImportService
    {
        private NSSMappingStorage storage;
        public NSSImportService(string connectionString) : base(connectionString)
        {
            storage = NSSMappingStorage.GetStorage();
        }

        public override void Import(byte[] data)
        {
            var nssConteiner = new AlMappingImportModel(data);
            CsvContainer = nssConteiner;
            var standards = ServiceLocatorMaster.CommonCoreStandardService.GetStandards();
            ISet<Guid> stIds = new HashSet<Guid>();
            foreach (var standard in standards) stIds.Add(standard.Id);


            for (int i = 0; i < nssConteiner.Rows.Count; i++)
            {
                if (!nssConteiner.ValideteCellHasValue(i, nssConteiner.SecondColumn, $"no {nssConteiner.SecondColumn} "))
                    continue;
                if (!nssConteiner.ValideteCellHasValue(i, nssConteiner.FirstColumn, $"no {nssConteiner.FirstColumn} "))
                    continue;

                Guid abTT1Id, ccId;
                if (!Guid.TryParse(nssConteiner.GetValue(i, nssConteiner.SecondColumn), out ccId))
                {
                    nssConteiner.Rows[i].State = CsvRowState.Failed;
                    nssConteiner.Rows[i].ErrorMessage = $"Invalid {nssConteiner.SecondColumn}";
                    continue;
                }
                if (!Guid.TryParse(nssConteiner.GetValue(i, nssConteiner.FirstColumn), out abTT1Id))
                {
                    nssConteiner.Rows[i].State = CsvRowState.Failed;
                    nssConteiner.Rows[i].ErrorMessage = $"Invalid {nssConteiner.FirstColumn}";
                    continue;
                }
                if (!stIds.Contains(ccId))
                {
                    nssConteiner.Rows[i].State = CsvRowState.Failed;
                    nssConteiner.Rows[i].ErrorMessage = $"There is no common core standard with such id  = '{ccId}'";
                    continue;
                }
                if (!storage.Contains(abTT1Id, ccId))
                {
                    storage.Add(abTT1Id, ccId);
                }
            }
        }
    }

    public class ImportAlMappingService : ImportService
    {
        private NSSMappingStorage storage;
        public ImportAlMappingService(string connectionString) : base(connectionString)
        {
            storage = NSSMappingStorage.GetStorage();
        }

        public override void Import(byte[] data)
        {
            var alConteiner = new AlMappingImportModel(data);
            CsvContainer = alConteiner;
            var mappings = ServiceLocatorMaster.CommonCoreStandardService.GetABToCCMappings(null, null);
            ISet<Pair<Guid, Guid>> addedMappings = new HashSet<Pair<Guid, Guid>>();
            foreach (var mapping in mappings)
            {
                addedMappings.Add(new Pair<Guid, Guid>(mapping.AcademicBenchmarkId, mapping.CCStandardRef));
            }

            var res = new List<ABToCCMapping>();
            for (int i = 0; i < alConteiner.Rows.Count; i++)
            {
                if (!alConteiner.ValideteCellHasValue(i, alConteiner.SecondColumn, $"no {alConteiner.SecondColumn} ")) continue;
                if (!alConteiner.ValideteCellHasValue(i, alConteiner.FirstColumn, $"no {alConteiner.FirstColumn} ")) continue;

                Guid abTTId, abId;

                if (!Guid.TryParse(alConteiner.GetValue(i, alConteiner.FirstColumn), out abTTId))
                {
                    alConteiner.Rows[i].State = CsvRowState.Failed;
                    alConteiner.Rows[i].ErrorMessage = $"Invalid {alConteiner.FirstColumn}";
                    continue;
                }

                var list = storage.GetByFirst(abTTId);

                if (list.Count == 0) continue;


                if (!Guid.TryParse(alConteiner.GetValue(i, alConteiner.SecondColumn), out abId))
                {
                    alConteiner.Rows[i].State = CsvRowState.Failed;
                    alConteiner.Rows[i].ErrorMessage = $"Invalid {alConteiner.SecondColumn}";
                    continue;
                }

                foreach (var elem in list)
                {
                    var pair = new Pair<Guid, Guid>(abId, elem.Second);
                    if (addedMappings.Contains(pair)) continue;

                    res.Add(new ABToCCMapping { AcademicBenchmarkId = abId, CCStandardRef = elem.Second });
                    addedMappings.Add(pair);
                }
            }
            ServiceLocatorMaster.CommonCoreStandardService.AddABToCCMapping(res);
        }
    }


    //public class ImportAlabamaMappingService
    //{
    //    protected IServiceLocatorMaster ServiceLocatorMaster { get; private set; }

    //    protected ImportAlabamaMappingService(string connectionString)
    //    {
    //        ServiceLocatorMaster = new StandardImportServiceLocatorMaster(connectionString);
    //    }

    //    public void Import(StandardMappingImportModel nssConteiner, StandardMappingImportModel alConteiner)
    //    {
    //        var standards = ServiceLocatorMaster.CommonCoreStandardService.GetStandards();
    //        ISet<Guid> stIds = new HashSet<Guid>();
    //        foreach (var standard in standards) stIds.Add(standard.Id);


    //        var mappings = ServiceLocatorMaster.CommonCoreStandardService.GetABToCCMappings(null, null);
    //        ISet<Pair<Guid, Guid>> addedMappings = new HashSet<Pair<Guid, Guid>>();
    //        foreach (var mapping in mappings)
    //        {
    //            addedMappings.Add(new Pair<Guid, Guid>(mapping.AcademicBenchmarkId, mapping.CCStandardRef));
    //        }

    //        var res = new List<ABToCCMapping>();
    //        for (int i = 0; i < nssConteiner.Rows.Count; i++)
    //        {
    //            if (!nssConteiner.ValideteCellHasValue(i, nssConteiner.SecondColumn, $"no {nssConteiner.SecondColumn} ")) continue;
    //            if (!nssConteiner.ValideteCellHasValue(i, nssConteiner.FirstColumn, $"no {nssConteiner.FirstColumn} ")) continue;

    //            Guid abTT1Id, ccId;
    //            if (!Guid.TryParse(nssConteiner.GetValue(i, nssConteiner.SecondColumn), out ccId))
    //            {
    //                nssConteiner.Rows[i].State = CsvRowState.Failed;
    //                nssConteiner.Rows[i].ErrorMessage = $"Invalid {nssConteiner.SecondColumn}";
    //                continue;
    //            }
    //            if (!Guid.TryParse(nssConteiner.GetValue(i, nssConteiner.FirstColumn), out abTT1Id))
    //            {
    //                nssConteiner.Rows[i].State = CsvRowState.Failed;
    //                nssConteiner.Rows[i].ErrorMessage = $"Invalid {nssConteiner.FirstColumn}";
    //                continue;
    //            }
    //            if (!stIds.Contains(ccId))
    //            {
    //                nssConteiner.Rows[i].State = CsvRowState.Failed;
    //                nssConteiner.Rows[i].ErrorMessage = $"There is no common core standard with such id  = '{ccId}'";
    //                continue;
    //            }

    //            for (int j = 0; i < alConteiner.Rows.Count; i++)
    //            {
    //                if (!alConteiner.ValideteCellHasValue(i, alConteiner.SecondColumn, $"no {alConteiner.SecondColumn} ")) continue;
    //                if (!alConteiner.ValideteCellHasValue(i, alConteiner.FirstColumn, $"no {alConteiner.FirstColumn} ")) continue;

    //                Guid abTT2Id, abId;

    //                if (!Guid.TryParse(nssConteiner.GetValue(i, nssConteiner.FirstColumn), out abTT2Id))
    //                {
    //                    nssConteiner.Rows[i].State = CsvRowState.Failed;
    //                    nssConteiner.Rows[i].ErrorMessage = $"Invalid {nssConteiner.FirstColumn}";
    //                    continue;
    //                }

    //                if(abTT1Id != abTT2Id) continue;

    //                if (!Guid.TryParse(alConteiner.GetValue(i, nssConteiner.SecondColumn), out abId))
    //                {
    //                    alConteiner.Rows[i].State = CsvRowState.Failed;
    //                    alConteiner.Rows[i].ErrorMessage = $"Invalid {alConteiner.SecondColumn}";
    //                    continue;
    //                }

    //                var pair = new Pair<Guid, Guid>(abId, ccId);
    //                if (addedMappings.Contains(pair)) continue;

    //                res.Add(new ABToCCMapping { AcademicBenchmarkId = abId, CCStandardRef = ccId });
    //                addedMappings.Add(pair);
    //            }
    //        }
    //        ServiceLocatorMaster.CommonCoreStandardService.AddABToCCMapping(res);
    //    }
    //}
}
