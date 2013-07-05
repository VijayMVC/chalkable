using System;
using System.Collections.Generic;
using System.Security;
using Chalkable.BusinessLogic.Mapping;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;


namespace Chalkable.BusinessLogic.Services.School
{
    public interface IGradingStyleService
    {
        IGradingStyleMapper GetMapper();
        void SetMapper(IGradingStyleMapper mapper);
        IList<GradingStyle> GetGradingStyles();
    }

    public class GradingStyleService : SchoolServiceBase, IGradingStyleService
    {
        public GradingStyleService(IServiceLocatorSchool serviceLocator)
            : base(serviceLocator)
        {
        }

        public IGradingStyleMapper GetMapper()
        {
            var list = GetGradingStyles();
            if (list.Count == 0)
                return GradingStyleMapper.CreateDefault();
            return GradingStyleMapper.Create(list);
        }

        public void SetMapper(IGradingStyleMapper mapper)
        {
            if (!BaseSecurity.IsAdminEditor(Context))
                throw new ChalkableSecurityException();

            using (var uow = Update())
            {
                var da = new GradingStyleDataAccess(uow);
                da.DeleteAll();
                var res = new List<GradingStyle>();
                res.AddRange(AddGradingStyleItems(GradingStyleEnum.Abcf, mapper.GetValuesByStyle(GradingStyleEnum.Abcf)));
                res.AddRange(AddGradingStyleItems(GradingStyleEnum.Complete, mapper.GetValuesByStyle(GradingStyleEnum.Complete)));
                res.AddRange(AddGradingStyleItems(GradingStyleEnum.Check, mapper.GetValuesByStyle(GradingStyleEnum.Check)));
                da.Insert(res);
                uow.Commit();
            }

        }

        public IList<GradingStyle> GetGradingStyles()
        {
            if (!BaseSecurity.IsAdminOrTeacher(Context))
                throw new ChalkableSecurityException();
            using (var uow = Read())
            {
                return new GradingStyleDataAccess(uow).GetAll();
            }
        }

        private IEnumerable<GradingStyle> AddGradingStyleItems(GradingStyleEnum style, IList<int> values)
        {
            var res = new List<GradingStyle>();
            for (int i = 0; i < values.Count; i++)
                res.Add(new GradingStyle()
                {
                    GradingStyleValue = style,
                    MaxValue = values[i],
                    StyledValue = i
                });
            return res;
        }



    }
}
