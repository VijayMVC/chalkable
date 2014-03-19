using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Model;
using Chalkable.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoAlphaGradeStorage:BaseDemoStorage
    {
        private readonly Dictionary<int ,AlphaGrade> alphaGradesData = new Dictionary<int, AlphaGrade>();

        public DemoAlphaGradeStorage(DemoStorage storage) : base(storage)
        {
        }

        public void Add(IList<AlphaGrade> alphaGrades)
        {
            foreach (var alphaGrade in alphaGrades)
            {
                if (alphaGradesData.ContainsKey(alphaGrade.Id))
                {
                    alphaGradesData[alphaGrade.Id] = alphaGrade;
                }
            }
        }

        public void Delete(int id)
        {
            alphaGradesData.Remove(id);
        }

        public IList<AlphaGrade> GetAll()
        {
            return alphaGradesData.Select(x => x.Value).ToList();
        }

        public void Delete(IList<int> ids)
        {
            foreach (var id in ids)
            {
                Delete(id);
            }
        }

        public void Update(IList<AlphaGrade> alphaGrades)
        {
            foreach (var alphaGrade in alphaGrades)
            {
                if (alphaGradesData.ContainsKey(alphaGrade.Id))
                {
                    alphaGradesData[alphaGrade.Id] = alphaGrade;
                }
            }
        }
    }
}
