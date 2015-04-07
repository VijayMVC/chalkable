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
                res = res.Union(standards.Where(s => (!string.IsNullOrEmpty(s.Name) && s.Name.IndexOf(word, StringComparison.OrdinalIgnoreCase) >= 0)
                                           || (!string.IsNullOrEmpty(s.CCStandardCode) && s.Name.IndexOf(word, StringComparison.OrdinalIgnoreCase) >= 0)
                                           || (!string.IsNullOrEmpty(s.Description) && s.Name.IndexOf(word, StringComparison.OrdinalIgnoreCase) >= 0)
                                ));
            }
            return res.ToList();
        }

        public StandardTreePath GetStandardParentsSubTree(int standardId)
        {
            int? currentParentId = standardId;
            Standard currentStandard;
            var allStandards = new List<Standard>();
            var standards = GetData().Select(x => x.Value).ToList();
            var res = new StandardTreePath
                {
                    AllStandards = new List<Standard>(),
                    Path = new List<Standard>()
                };
            var lastChild = standards.Where(x => x.ParentStandardRef == currentParentId).ToList();
            allStandards.AddRange(lastChild);
            while (currentParentId.HasValue)
            {
                currentStandard = GetById(currentParentId.Value);
                currentParentId = currentStandard.ParentStandardRef;
                allStandards.AddRange(currentParentId.HasValue
                                          ? standards.Where(x => x.ParentStandardRef == currentParentId).ToList()
                                          : standards.Where(x => !x.ParentStandardRef.HasValue && x.StandardSubjectRef == currentStandard.StandardSubjectRef).ToList());
                res.Path.Add(currentStandard);
            }
            res.Path = res.Path.Reverse().ToList(); 
            res.AllStandards = allStandards;
            return res;
        }

    }
}
