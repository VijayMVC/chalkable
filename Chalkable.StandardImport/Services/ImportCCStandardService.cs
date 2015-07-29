using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.Master.Model;
using Chalkable.StandardImport.Models;

namespace Chalkable.StandardImport.Services
{
    public class ImportCCStandardService : ImportService
    {
        public ImportCCStandardService(string connectionString) : base(connectionString)
        {
        }

        public override void Import(byte[] data)
        {
            CsvContainer = new CCStandardImportModel(data);
            ImportCategories();
            ImportStandards();
        }

        private void ImportCategories()
        {
            var addedCategories = ServiceLocatorMaster.CommonCoreStandardService.GetCCStandardCategories();
            var sources = CsvContainer.ColumnValuesDistinct(CsvContainer.GetSourceColumnIndex(CCStandardImportModel.CATEGORY_COLUMN_NAME));
            sources = sources.Where(x => addedCategories.All(y => y.Name.ToLower() != x.ToLower())).ToList();
            var categories = sources.Select(x => new CommonCoreStandardCategory
            {
                Id = Guid.NewGuid(),
                Name = x
            }).ToList();
            ServiceLocatorMaster.CommonCoreStandardService.AddStandardsCategories(categories);
        }
        private void ImportStandards()
        {
            var categories = ServiceLocatorMaster.CommonCoreStandardService.GetCCStandardCategories();
            var addedStandards = ServiceLocatorMaster.CommonCoreStandardService.GetStandards();
            ISet<Guid> addedStandardsIds = new HashSet<Guid>();
            foreach (var addedStandard in addedStandards) 
                addedStandardsIds.Add(addedStandard.Id);
            var res = new List<CommonCoreStandard>();
            for (int i = 0; i < CsvContainer.Rows.Count; i++)
                AddStandard(res, categories, i, addedStandardsIds);
            ServiceLocatorMaster.CommonCoreStandardService.AddStandards(res);
        }

        private void AddStandard(IList<CommonCoreStandard> standards, IList<CommonCoreStandardCategory> categories, int rowIndex, ISet<Guid> addedStandards)
        {
            var idField = CCStandardImportModel.ID_COLUMN_NAME;
            if (!CsvContainer.ValideteCellHasValue(rowIndex, idField, "no id field")) return;

            Guid id, parentId;
            if (!Guid.TryParse(CsvContainer.GetValue(rowIndex, idField), out id))
            {
                CsvContainer.Rows[rowIndex].State = CsvRowState.Failed;
                CsvContainer.Rows[rowIndex].ErrorMessage = "Invalid standard Id";
                return;
            }
            if (!addedStandards.Contains(id))
            {
                var categoryField = CCStandardImportModel.CATEGORY_COLUMN_NAME;
                if (!CsvContainer.ValideteCellHasValue(rowIndex, categoryField, "no subject field")) return;

                var category = categories.First(x => x.Name.ToLower() == CsvContainer.GetValue(rowIndex, categoryField).ToLower());
                var standard = new CommonCoreStandard
                {
                    Code = CsvContainer.GetValue(rowIndex, CCStandardImportModel.STANDARD_COLUMN_NAME),
                    Description = CsvContainer.GetValue(rowIndex, CCStandardImportModel.DESCRIPTION_COLUMN_NAME),
                    Id = id,
                    StandardCategoryRef = category.Id
                };

                var parentIdStr = CsvContainer.GetValue(rowIndex, CCStandardImportModel.PARENT_ID_COLUMN_NAME);
                if (!string.IsNullOrEmpty(parentIdStr))
                {
                    if (!Guid.TryParse(parentIdStr, out parentId))
                    {
                        CsvContainer.Rows[rowIndex].State = CsvRowState.Failed;
                        CsvContainer.Rows[rowIndex].ErrorMessage = "Invalid standard parent Id";
                        return;
                    }
                    if (!addedStandards.Contains(parentId))
                    {
                        var parentIdCoumnIndex = CsvContainer.GetSourceColumnIndex(CCStandardImportModel.PARENT_ID_COLUMN_NAME);
                        var row = CsvContainer.Rows.FirstOrDefault(x => x.Items[parentIdCoumnIndex] == parentId.ToString().ToLower());
                        if (row == null)
                        {
                            CsvContainer.Rows[rowIndex].State = CsvRowState.Failed;
                            CsvContainer.Rows[rowIndex].ErrorMessage = string.Format("Invalid parent id. Standard with such id = '{0}' doesn't exists in current document ", parentId);
                            return;
                        }
                        AddStandard(standards, categories, CsvContainer.Rows.IndexOf(row), addedStandards);
                    }
                    standard.ParentStandardRef = parentId;
                }
                standards.Add(standard);
                addedStandards.Add(standard.Id);
            }
        }
    }

}
