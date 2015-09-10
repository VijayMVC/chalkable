using System;
using System.Collections.Generic;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.Model;
using Chalkable.Data.School.Model.Announcements;
using Chalkable.StiConnector.Connectors.Model;

namespace Chalkable.BusinessLogic.Mapping.ModelMappers
{
    public class MapperFactory
    {
        private static IDictionary<Pair<Type, Type>, Object> _customMappers;

        static MapperFactory()
        {
            BuildMapperDictionary();
        }

        private static void BuildMapperDictionary()
        {
            _customMappers = new Dictionary<Pair<Type, Type>, Object>
                    {
                        {new Pair<Type, Type>(typeof(Announcement), typeof(Activity)), new ActivityToAnnouncementMapper()},
                        {new Pair<Type, Type>(typeof(ClassAnnouncement), typeof(Activity)), new ActivityToClassAnnouncementMapper()},
                        {new Pair<Type, Type>(typeof(AnnouncementComplex), typeof(Activity)), new ActivityToAnnouncementComplexMapper()},
                        {new Pair<Type, Type>(typeof(AnnouncementDetails), typeof(Activity)), new ActivityToAnnouncementDetailsMapper()},

                        {new Pair<Type, Type>(typeof(Activity), typeof(Announcement)), new AnnouncementToActivityMapper()},
                        {new Pair<Type, Type>(typeof(Activity), typeof(ClassAnnouncement)), new ClassAnnouncementToActivityMapper()},
                        {new Pair<Type, Type>(typeof(Activity), typeof(AnnouncementComplex)), new AnnouncementComplexToActivityMapper()},
                        {new Pair<Type, Type>(typeof(Activity), typeof(AnnouncementDetails)), new AnnouncementDetailsToActivityMapper()},

                        {new Pair<Type, Type>(typeof(StudentAnnouncement), typeof(Score)), new ScoreToStudentAnnMapper()},
                        {new Pair<Type, Type>(typeof(StudentAnnouncementDetails), typeof(Score)), new ScoreToStudentAnnMapper()},
                        {new Pair<Type, Type>(typeof(Score), typeof(StudentAnnouncementDetails)), new StudentAnnouncementToScoreMapper()},
                        {new Pair<Type, Type>(typeof(Score), typeof(StudentAnnouncement)), new StudentAnnouncementToScoreMapper()},
                        {new Pair<Type, Type>(typeof(ClassDiscipline), typeof(DisciplineReferral)), new DisciplineReferralToClassDisciplineMapper()},
                        {new Pair<Type, Type>(typeof(DisciplineReferral), typeof(ClassDiscipline)), new ClassDisciplineToDisciplineReferralMapper()},
                        {new Pair<Type, Type>(typeof(AnnouncementAssignedAttribute), typeof(ActivityAssignedAttribute)), new ActivityAssignedAttrToAnnouncementAssignedAttrMapper() },
                        {new Pair<Type, Type>(typeof(ActivityAssignedAttribute), typeof(AnnouncementAssignedAttribute)), new AnnouncementAssignedAttrToActivityAssignedAttrMapper() },
                    
                        {new Pair<Type, Type>(typeof(Attachment), typeof(StiAttachment)), new StiAttachmentToAttachmentMapper()},
                        {new Pair<Type, Type>(typeof(StiAttachment), typeof(Attachment)), new AttachmentToStiAttachmentMapper()}
                    };
        }
        public static IMapper<TReturn, TSource> GetMapper<TReturn, TSource>()
        {
            var typesObj = new Pair<Type, Type>(typeof(TReturn), typeof(TSource));
            if (!_customMappers.ContainsKey(typesObj))
                throw new ChalkableException("There are no mapper with such source and return types");
            return _customMappers[typesObj] as IMapper<TReturn, TSource>;
        }
    }
}
