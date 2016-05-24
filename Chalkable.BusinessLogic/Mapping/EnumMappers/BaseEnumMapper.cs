using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Common.Exceptions;

namespace Chalkable.BusinessLogic.Mapping.EnumMappers
{
    public interface IEnumMapper<TEnum1, TEnum2>
    {
        TEnum2 Map(TEnum1 from);
        TEnum1 MapBack(TEnum2 from);
    }

    public abstract class BaseEnumMapper<TEnum1, TEnum2> : IEnumMapper<TEnum1, TEnum2> 
        where TEnum1 : IComparable 
        where TEnum2 : IComparable
    {
        protected static Dictionary<TEnum1, TEnum2> _mapperDictionary;
        
        private void ValidateEnumType()
        {
            if (!typeof (TEnum1).IsEnum || !typeof (TEnum2).IsEnum)
                throw new ChalkableException("TEnum1 and TEnum2 must be an enum type");
        }
        public TEnum2 Map(TEnum1 from)
        {
            ValidateEnumType();
            return InternalMap(from);
        }

        protected virtual TEnum2 InternalMap(TEnum1 from)
        {
            if (!_mapperDictionary.ContainsKey(from))
                throw new ChalkableException();
            return _mapperDictionary[from];
        }

        public TEnum1 MapBack(TEnum2 from)
        {
            ValidateEnumType();
            return InternalMapBack(from);
        }

        protected virtual TEnum1 InternalMapBack(TEnum2 from)
        {
            if(!_mapperDictionary.ContainsValue(from))
                throw new ChalkableException("Can't find");

            return _mapperDictionary.First(x => from.CompareTo(x.Value) == 0).Key;
        }
    }
}
