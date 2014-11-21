using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Common;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Master.Model;
using Chalkable.Data.School.Model;
using Chalkable.Web.Models.PersonViewDatas;

namespace Chalkable.Web.Models
{
    public class SearchViewData
    {
        public string Id { get; set; }
        public string Description { get; set; }
        public int SearchType { get; set; }

        public static IList<SearchViewData> Create(IDictionary<SearchTypeEnum, Object> searchResult)
        {
            IDictionary<SearchTypeEnum, BaseSearchResultBuilder> mapper = new Dictionary<SearchTypeEnum, BaseSearchResultBuilder>();
            var res = new List<SearchViewData>();
            PreperingBuilderMapper(mapper);
            foreach (var searchValue in searchResult)
            {
                res.AddRange(mapper[searchValue.Key].Build(searchValue.Value));
            }
            return res;
        }

        private static void PreperingBuilderMapper(IDictionary<SearchTypeEnum, BaseSearchResultBuilder> mapper)
        {
            mapper.Add(SearchTypeEnum.Persons, new SearchPersonBuilder(SearchTypeEnum.Persons));
            mapper.Add(SearchTypeEnum.Applications, new SearchApplicationBuilder(SearchTypeEnum.Applications));
            mapper.Add(SearchTypeEnum.Announcements, new SearchAnnouncementBuilder(SearchTypeEnum.Announcements));
            mapper.Add(SearchTypeEnum.Attachments, new SearchAttachmentBuilder(SearchTypeEnum.Attachments));
            mapper.Add(SearchTypeEnum.Classes, new SearchClassBuilder(SearchTypeEnum.Classes));
        }
    }


    public class PersonSearchViewData : SearchViewData
    {
        public ShortPersonViewData ShortPersonInfo { get; set; }
        
        public static SearchViewData Create(Person person)
        {
            return new PersonSearchViewData
            {
                Description = person.FullName(),
                Id = person.Id.ToString(),
                SearchType = (int)SearchTypeEnum.Persons,
                ShortPersonInfo = ShortPersonViewData.Create(person)
            };
        }
    }
    public class ApplicationSearchViewData : SearchViewData
    {
        public Guid? SmallPictureId { get; set; }
        public Guid? BigPictureId { get; set; }

        public static SearchViewData Create(Application application)
        {
            return new ApplicationSearchViewData
            {
                Id = application.Id.ToString(),
                Description = application.Name,
                SmallPictureId = application.SmallPictureRef,
                BigPictureId = application.BigPictureRef,
                SearchType = (int)SearchTypeEnum.Applications
            };
        }
    }
    public class AnnouncementSearchViewData : SearchViewData
    {
        public int? AnnouncementType { get; set; }
        public bool IsAdminAnnouncement { get; set; }
        public static SearchViewData Create(AnnouncementComplex announcement)
        {
            return new AnnouncementSearchViewData
            {
                Id = announcement.Id.ToString(),
                Description = string.Format("{0} {1} {2}", announcement.Title, announcement.ClassAnnouncementTypeName, announcement.Order),
                AnnouncementType = announcement.ChalkableAnnouncementType,
                IsAdminAnnouncement = !announcement.ClassAnnouncementTypeRef.HasValue,
                SearchType = (int)SearchTypeEnum.Announcements
            };
        }
    }
    public class AttachmentSearchViewData : SearchViewData
    {
        public int AnnouncementId { get; set; }
        public static SearchViewData Create(AnnouncementAttachment attachment)
        {
            return new AttachmentSearchViewData
            {
                Id = attachment.Id.ToString(),
                Description = attachment.Name,
                AnnouncementId = attachment.AnnouncementRef,
                SearchType = (int)SearchTypeEnum.Attachments
            };
        }
    }

    public class ClassSearchViewData : SearchViewData
    {
        public Guid? DepartmentId { get; set; }
        //public  Course { get; set; }
        public static SearchViewData Create(ClassDetails cClass)
        {
            return new ClassSearchViewData
            {
                Id = cClass.Id.ToString(),
                Description = cClass.Name,
                DepartmentId = cClass.ChalkableDepartmentRef,
                SearchType = (int)SearchTypeEnum.Classes,
                //Course = CourseViewData.Create(cClass.Course)
            };
        }
    }


    public abstract class BaseSearchResultBuilder
    {
        protected int searchType;
        public BaseSearchResultBuilder(SearchTypeEnum searchTypeEnum)
        {
            searchType = (int)searchTypeEnum;
        }

        public abstract IList<SearchViewData> Build(Object searchRes);
    }
    public class SearchPersonBuilder : BaseSearchResultBuilder
    {
        public SearchPersonBuilder(SearchTypeEnum searchTypeEnum)
            : base(searchTypeEnum)
        {
        }

        public override IList<SearchViewData> Build(Object searchRes)
        {
            var schoolPersons = searchRes as IList<Person>;
            if (schoolPersons == null || (SearchTypeEnum)searchType != SearchTypeEnum.Persons)
                throw new ChalkableException("Invalid search View Builder for such search type");

            return schoolPersons.Select(PersonSearchViewData.Create).ToList();
        }
    }
    public class SearchApplicationBuilder : BaseSearchResultBuilder
    {
        public SearchApplicationBuilder(SearchTypeEnum searchTypeEnum)
            : base(searchTypeEnum)
        {
        }

        public override IList<SearchViewData> Build(Object searchRes)
        {
            var applications = searchRes as IList<Application>;
            if (applications == null || (SearchTypeEnum)searchType != SearchTypeEnum.Applications)
                throw new ChalkableException(ChlkResources.ERR_INVALID_SEARCH_VIEW_BUILDER);

            return applications.Select(ApplicationSearchViewData.Create).ToList();
        }
    }
    public class SearchAnnouncementBuilder : BaseSearchResultBuilder
    {
        public SearchAnnouncementBuilder(SearchTypeEnum searchTypeEnum)
            : base(searchTypeEnum)
        {
        }

        public override IList<SearchViewData> Build(Object searchRes)
        {
            var announcements = searchRes as IList<AnnouncementComplex>;
            if (announcements == null || (SearchTypeEnum)searchType != SearchTypeEnum.Announcements)
                throw new ChalkableException(ChlkResources.ERR_INVALID_SEARCH_VIEW_BUILDER);

            return announcements.Select(AnnouncementSearchViewData.Create).ToList();
        }
    }
    public class SearchAttachmentBuilder : BaseSearchResultBuilder
    {
        public SearchAttachmentBuilder(SearchTypeEnum searchTypeEnum)
            : base(searchTypeEnum)
        {
        }

        public override IList<SearchViewData> Build(Object searchRes)
        {
            var attachment = searchRes as IList<AnnouncementAttachment>;
            if (attachment == null || (SearchTypeEnum)searchType != SearchTypeEnum.Attachments)
                throw new ChalkableException(ChlkResources.ERR_INVALID_SEARCH_VIEW_BUILDER);

            return attachment.Select(AttachmentSearchViewData.Create).ToList();
        }
    }
    public class SearchClassBuilder : BaseSearchResultBuilder
    {
        public SearchClassBuilder(SearchTypeEnum searchTypeEnum)
            : base(searchTypeEnum)
        {
        }

        public override IList<SearchViewData> Build(object searchRes)
        {
            var classes = searchRes as IList<ClassDetails>;
            if (classes == null || (SearchTypeEnum)searchType != SearchTypeEnum.Classes)
                throw new ChalkableException(ChlkResources.ERR_INVALID_SEARCH_VIEW_BUILDER);
            return classes.Select(ClassSearchViewData.Create).ToList();
        }
    }


    public enum SearchTypeEnum
    {
        Persons = 0,
        Applications = 1,
        Announcements = 2,
        Attachments = 3,
        Classes = 4
    }
}