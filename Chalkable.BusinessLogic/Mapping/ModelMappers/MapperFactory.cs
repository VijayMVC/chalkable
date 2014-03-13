using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.Model;
using Chalkable.StiConnector.Connectors.Model;

namespace Chalkable.BusinessLogic.Mapping.ModelMappers
{
    public class MapperFactory
    {
        private static IDictionary<Pair<Type, Type>, IMapper> _customMappers;

        static MapperFactory()
        {
            BuildMapperDictionary();
        }

        private static void BuildMapperDictionary()
        {
            _customMappers = new Dictionary<Pair<Type, Type>, IMapper>
                    {
                        {new Pair<Type, Type>(typeof(Announcement), typeof(Activity)), new ActivityToAnnouncementMapper()},
                        {new Pair<Type, Type>(typeof(AnnouncementComplex), typeof(Activity)), new ActivityToAnnouncementMapper()},
                        {new Pair<Type, Type>(typeof(AnnouncementDetails), typeof(Activity)), new ActivityToAnnouncementMapper()},
                        {new Pair<Type, Type>(typeof(Activity), typeof(AnnouncementComplex)), new AnnouncementComplexToActivityMapper()},
                        {new Pair<Type, Type>(typeof(Activity), typeof(AnnouncementDetails)), new AnnouncementComplexToActivityMapper()},
                        {new Pair<Type, Type>(typeof(ActivityAttachment), typeof(AnnouncementAttachment)), new AnnouncementAttToActivityAttMapper()},
                        {new Pair<Type, Type>(typeof(AnnouncementAttachment), typeof(ActivityAttachment)), new StiAttachmentToAnnouncementAttMapper()},
                        {new Pair<Type, Type>(typeof(AnnouncementAttachment), typeof(StiAttachment)), new StiAttachmentToAnnouncementAttMapper()},
                        {new Pair<Type, Type>(typeof(StudentAnnouncement), typeof(Score)), new ScoreToStudentAnnMapper()},
                        {new Pair<Type, Type>(typeof(StudentAnnouncementDetails), typeof(Score)), new ScoreToStudentAnnMapper()},
                        {new Pair<Type, Type>(typeof(Score), typeof(StudentAnnouncementDetails)), new StudentAnnouncementToScoreMapper()},
                        {new Pair<Type, Type>(typeof(Score), typeof(StudentAnnouncement)), new StudentAnnouncementToScoreMapper()},
                    };
        }
        public static IMapper GetMapper<TReturn, TSource>()
        {
            var typesObj = new Pair<Type, Type>(typeof(TReturn), typeof(TSource));
            if (!_customMappers.ContainsKey(typesObj))
                throw new ChalkableException("There are no mapper with such source and return types");
            return _customMappers[typesObj];
        }
    }
}
