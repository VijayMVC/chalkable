using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.StiConnector.Connectors;

namespace Chalkable.BusinessLogic.Mapping.EnumMappers
{
    public static class EnumMapperFactory
    {
        private static IDictionary<Pair<Type, Type>, object> _customEnumMappers;

        static EnumMapperFactory()
        {
            _customEnumMappers = new Dictionary<Pair<Type, Type>, object>
            {
                {new Pair<Type, Type>(typeof(ClassSortType), typeof(SectionSummarySortOption)), new ClassSortTypeToSectionSummarySortOpt()},
                {new Pair<Type, Type>(typeof(TeacherSortType), typeof(SectionSummarySortOption)), new TeacherSortTypeToSectionSummarySortOpt()}
            };
        }

        public static IEnumMapper<TEnum1, TEnum2> GetMapper<TEnum1, TEnum2>()
        {
            var typesObj = new Pair<Type, Type>(typeof(TEnum1), typeof(TEnum2));
            if(!_customEnumMappers.ContainsKey(typesObj))
                throw new ChalkableException("There are no mapper with such first and second enum");

            return _customEnumMappers[typesObj] as IEnumMapper<TEnum1, TEnum2>;
        }
    }
}
