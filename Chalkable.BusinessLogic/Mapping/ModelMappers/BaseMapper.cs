namespace Chalkable.BusinessLogic.Mapping.ModelMappers
{
    public interface IMapper<in TReturn, in TSource>
    {
        void Map(TReturn returnObj, TSource sourceObj);
    }

    public abstract class BaseMapper<TReturn, TSource> : IMapper<TReturn, TSource>
        where TReturn : class
        where TSource : class
    {
        public void Map(TReturn returnObj, TSource sourceObj)
        {
            InnerMap(returnObj, sourceObj);
        }
        protected abstract void InnerMap(TReturn returnObj, TSource sourceObj);
    }
}
