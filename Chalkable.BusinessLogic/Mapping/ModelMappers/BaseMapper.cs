using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Common.Exceptions;

namespace Chalkable.BusinessLogic.Mapping.ModelMappers
{
    public interface IMapper
    {
        void Map(Object returnObj, Object sourceObj);
    }

    public abstract class BaseMapper<TReturn, TSource> : IMapper
        where TReturn : class
        where TSource : class
    {
        public void Map(object returnObj, object sourceObj)
        {
            var obj1 = returnObj as TReturn;
            var obj2 = sourceObj as TSource;
            if (obj1 == null || obj2 == null)
                throw new ChalkableException("Invalid param type");
            InnerMap(obj1, obj2);
        }
        protected abstract void InnerMap(TReturn returnObj, TSource sourceObj);
    }
}
