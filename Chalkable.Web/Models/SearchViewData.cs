using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Common;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Master.Model;
using Chalkable.Data.School.Model;
using Chalkable.Data.School.Model.Announcements;
using Chalkable.Web.Controllers;
using Chalkable.Web.Logic;
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

        public static SearchViewData Create(Staff staff)
        {
            return new PersonSearchViewData
            {
                Description = staff.FullName(),
                Id = staff.Id.ToString(),
                SearchType = (int)SearchTypeEnum.Persons,
                ShortPersonInfo = StaffViewData.Create(staff)
            };
        }

        public static SearchViewData Create(Student student)
        {
            return new PersonSearchViewData
            {
                Description = student.FullName(),
                Id = student.Id.ToString(),
                SearchType = (int)SearchTypeEnum.Persons,
                ShortPersonInfo = StudentViewData.Create(student)
            };
        }
    }

    public class AnnouncementSearchViewData : SearchViewData
    {
        public int? AnnouncementType { get; set; }
        public bool IsAdminAnnouncement { get; set; }
        public static SearchViewData Create(Announcement announcement)
        {
            return new AnnouncementSearchViewData
            {
                Id = announcement.Id.ToString(),
                Description = announcement.Title,
                AnnouncementType = (int?)announcement.Type,
                SearchType = (int)SearchTypeEnum.Announcements
            };
        }
    }
    public class AttachmentSearchViewData : SearchViewData
    {
        public int AnnouncementId { get; set; }
        public string AttachmentThumbnailUrl { get; set; }
       
        public static SearchViewData Create(AnnouncementAttachmentInfo info)
        {
            return new AttachmentSearchViewData
            {
                Id = info.AttachmentInfo.Attachment.Id.ToString(),
                Description = info.AttachmentInfo.Attachment.Name,
                AnnouncementId = info.AnnouncementAttachment.AnnouncementRef,
                SearchType = (int)SearchTypeEnum.Attachments,
                AttachmentThumbnailUrl = info.AttachmentInfo.DownloadThumbnailUrl
            };
        }
    }

    public class ClassSearchViewData : SearchViewData
    {
        public Guid? DepartmentId { get; set; } 
        public static SearchViewData Create(ClassDetails cClass)
        {
            return new ClassSearchViewData
            {
                Id = cClass.Id.ToString(),
                Description = cClass.Name,
                DepartmentId = cClass.ChalkableDepartmentRef,
                SearchType = (int)SearchTypeEnum.Classes,
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
            var personList = searchRes as SearchController.PersonList;
            if (personList == null || (SearchTypeEnum)searchType != SearchTypeEnum.Persons)
                throw new ChalkableException("Invalid search View Builder for such search type");

            var res = new List<SearchViewData>();
            if(personList.Students.Count > 0)
                res.AddRange(personList.Students.Select(PersonSearchViewData.Create));
            if (personList.Staffs.Count > 0)
                res.AddRange(personList.Staffs.Select(PersonSearchViewData.Create));
            return res;
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
            var announcements = searchRes as IList<Announcement>;
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
            var attachment = searchRes as IList<AnnouncementAttachmentInfo>;
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
        Classes = 4,
    }
}