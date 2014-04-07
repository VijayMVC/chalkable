﻿using System;
using Chalkable.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoDisciplineTypeStorage:BaseDemoStorage<Guid, DisciplineType>
    {
        public DemoDisciplineTypeStorage(DemoStorage storage) : base(storage)
        {
        }

        public DisciplineType Add(string name, int score)
        {
            var dp = new DisciplineType
            {
                Id = Guid.NewGuid(),
                Name = name,
                Score = score
            };
            data[dp.Id] = dp;
            return dp;
        }


        public PaginatedList<DisciplineType> GetDisciplineTypes(int start, int count)
        {
            throw new NotImplementedException();
        }

        public DisciplineType Edit(Guid id, string name, int score)
        {
            var dp = data.ContainsKey(id) ? GetById(id) : null;


            if (dp != null)
            {
                dp.Name = name;
                dp.Score = score;
            }

            return dp;
        }

    }
}
