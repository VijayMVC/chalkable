using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Common;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.Model;
using Chalkable.Data.School.Model.Announcements;
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
            IDictionary<SearchTypeEnum, ISearchBuilder> mapper = new Dictionary<SearchTypeEnum, ISearchBuilder>();
            var res = new List<SearchViewData>();
            PreperingBuilderMapper(mapper);
            foreach (var searchValue in searchResult)
                res.AddRange(mapper[searchValue.Key].Build(searchValue.Value));
            return res;
        }

        private static void PreperingBuilderMapper(IDictionary<SearchTypeEnum, ISearchBuilder> mapper)
        {
            mapper.Add(SearchTypeEnum.Student, new DefaultSearchResultBuilder<StudentSchoolsInfo>(PersonSearchViewData.Create));
            mapper.Add(SearchTypeEnum.Staff, new DefaultSearchResultBuilder<Staff>(PersonSearchViewData.Create));
            mapper.Add(SearchTypeEnum.Announcements, new DefaultSearchResultBuilder<Announcement>(AnnouncementSearchViewData.Create));
            mapper.Add(SearchTypeEnum.Attachments, new DefaultSearchResultBuilder<AnnouncementAttachmentInfo>(AttachmentSearchViewData.Create));
            mapper.Add(SearchTypeEnum.Classes, new DefaultSearchResultBuilder<ClassDetails>(ClassSearchViewData.Create));
            mapper.Add(SearchTypeEnum.Group, new DefaultSearchResultBuilder<Group>(GroupSearchViewData.Create));
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

    public class GroupSearchViewData : SearchViewData
    {
        public static SearchViewData Create(Group gGroup)
        {
            return new GroupSearchViewData
            {
                Id = gGroup.Id.ToString(),
                Description = gGroup.Name,
                SearchType = (int)SearchTypeEnum.Group
            };
        }
    }

    public interface ISearchBuilder
    {
        IList<SearchViewData> Build(object searchRes);
    }

    public class DefaultSearchResultBuilder<TSearchItem> : ISearchBuilder
    {
        protected Func<TSearchItem, SearchViewData> _dataCreator;
        public DefaultSearchResultBuilder(Func<TSearchItem, SearchViewData> dataCreator)
        {
            _dataCreator = dataCreator;
        }

        public virtual IList<SearchViewData> Build(object searchRes)
        {
            var reslist = searchRes as IList<TSearchItem>;
            if (reslist == null)
                throw new ChalkableException(ChlkResources.ERR_INVALID_SEARCH_VIEW_BUILDER);

            return reslist.Select(_dataCreator).ToList();
        }
    }


    public enum SearchTypeEnum
    {
        Persons = 0,
        Applications = 1,
        Announcements = 2,
        Attachments = 3,
        Classes = 4,
        Student = 5,
        Staff = 6,
        Group = 7
    }
}