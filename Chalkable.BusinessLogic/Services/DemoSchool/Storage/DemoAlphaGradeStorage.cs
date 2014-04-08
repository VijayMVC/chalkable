using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Model;
using Chalkable.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoAlphaGradeStorage:BaseDemoStorage<int, AlphaGrade>
    {
        public DemoAlphaGradeStorage(DemoStorage storage) : base(storage)
        {
        }

        public void Add(IList<AlphaGrade> alphaGrades)
        {
            foreach (var alphaGrade in alphaGrades)
            {
                if (!data.ContainsKey(alphaGrade.Id))
                {
                    data[alphaGrade.Id] = alphaGrade;
                }
            }
        }

        public void Update(IList<AlphaGrade> alphaGrades)
        {
            foreach (var alphaGrade in alphaGrades)
            {
                if (data.ContainsKey(alphaGrade.Id))
                {
                    data[alphaGrade.Id] = alphaGrade;
                }
            }
        }
    }
}
