using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Chalkable.BusinessLogic.Services.DemoSchool.Storage;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool
{
    public class DemoGradingCommentService : BaseDemoStorage<int, GradingComment>, IGradingCommentService
    {
        public DemoGradingCommentService(IServiceLocator serviceLocator, DemoStorage demoStorage) : base(demoStorage)
        {
        }

        public void Add(IList<GradingComment> gradingComments)
        {
            foreach (var gc in gradingComments)
            {
                gc.Id = GetNextFreeId();
                data.Add(gc.Id, gc);
            }
        }

        public void Edit(IList<GradingComment> gradingComments)
        {
            foreach (var gc in gradingComments)
            {
                if (data.ContainsKey(gc.Id))
                    data[gc.Id] = gc;
            }
        }

        public override void Setup()
        {
            throw new NotImplementedException();
        }

        public IList<GradingComment> GetGradingComments()
        {
            return data.Select(x => x.Value).ToList();
        }
    }
}
