using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Services.DemoSchool.Common;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoStandardStorage:BaseDemoIntStorage<Standard>
    {
        public DemoStandardStorage(DemoStorage storage) : base(storage, x => x.Id)
        {
        }

        public IList<Standard> GetStandarts(StandardQuery query)
        {
            var standards = data.Select(x => x.Value);
            if (query.StandardSubjectId.HasValue)
                standards = standards.Where(x => x.StandardSubjectRef == query.StandardSubjectId);
            if (query.GradeLavelId.HasValue)
                standards =
                    standards.Where(
                        x => query.GradeLavelId <= x.LowerGradeLevelRef && query.GradeLavelId >= x.UpperGradeLevelRef);
            if (!query.AllStandards || query.ParentStandardId.HasValue)
                standards = standards.Where(x => x.ParentStandardRef == query.ParentStandardId);

            if (query.ClassId.HasValue)
            {
                var classStandarts = Storage.ClassStandardStorage.GetAll(query.ClassId).Select(x => x.StandardRef);
                standards = standards.Where(x => classStandarts.Contains(x.Id));
            }
            return standards.ToList();
        }

        public IList<Standard> SearchStandards(string filter)
        {
            if (string.IsNullOrEmpty(filter)) return new List<Standard>();
            var words = filter.ToLower().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (words.Length == 0)
                return new List<Standard>();

            var standards = data.Select(x => x.Value).ToList();
            var res = (new List<Standard>()).AsEnumerable();
            for (var i = 0; i < words.Length; i++)
            {
                var word = words[i];
                res = res.Union(standards.Where(s => (!string.IsNullOrEmpty(s.Name) && word.IndexOf(s.Name, StringComparison.OrdinalIgnoreCase) >= 0)
                                           || (!string.IsNullOrEmpty(s.CCStandardCode) && word.IndexOf(s.CCStandardCode, StringComparison.OrdinalIgnoreCase) >= 0)
                                           || (!string.IsNullOrEmpty(s.Description) && word.IndexOf(s.Description, StringComparison.OrdinalIgnoreCase) >= 0)
                                ));
            }
            return res.ToList();
        }

        public IList<StandardTreeItem> GetStandardParentsSubTree(int standardId)
        {
            var currentStandard = GetById(standardId);
            var lastParent = GetParentWithChilds(currentStandard);
            var lastParents = GetData().Where(x => x.Value.StandardSubjectRef == lastParent.StandardSubjectRef
                                                   && x.Value.Id != lastParent.Id && !x.Value.ParentStandardRef.HasValue)
                                       .Select(s => CreateStandardTreeItem(s.Value)).ToList();
            var res = new List<StandardTreeItem> {lastParent};
            res.AddRange(lastParents);
            return res;
        }

        private StandardTreeItem GetParentWithChilds(Standard child)
        {
            if (!child.ParentStandardRef.HasValue)
                return child as StandardTreeItem ?? CreateStandardTreeItem(child);  

            var parent = GetById(child.ParentStandardRef.Value);
            var res = CreateStandardTreeItem(parent);
            res.StandardChildren = GetData()
                                          .Where(x => x.Value.ParentStandardRef == parent.Id)
                                          .Select(x => CreateStandardTreeItem(x.Value)).ToList();
            return GetParentWithChilds(res);
        }

        private StandardTreeItem CreateStandardTreeItem(Standard standard)
        {
            return new StandardTreeItem
            {
                Id = standard.Id,
                AcademicBenchmarkId = standard.AcademicBenchmarkId,
                CCStandardCode = standard.CCStandardCode,
                Description = standard.Description,
                IsActive = standard.IsActive,
                LowerGradeLevelRef = standard.LowerGradeLevelRef,
                Name = standard.Name,
                ParentStandardRef = standard.ParentStandardRef,
                StandardChildren = new List<StandardTreeItem>(),
                StandardSubjectRef = standard.StandardSubjectRef
            };
        }


    }
}
