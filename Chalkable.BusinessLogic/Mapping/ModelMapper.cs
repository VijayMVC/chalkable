using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.Model;
using Chalkable.StiConnector.Connectors.Model;

namespace Chalkable.BusinessLogic.Mapping
{

    public interface IModelMapper
    {
        void Map<TReturn, TSource>(TReturn obj1, TSource obj2)
            where TReturn : class
            where TSource : class;
    }

    public interface IMapper
    {
        void Map(Object returnObj, Object sourceObj);
    }

    public class ActivityToAnnouncementMapper : IMapper
    {
        public virtual void Map(object returnObj, object sourceObj)
        {
            var ann = returnObj as Announcement;
            var activity = sourceObj as Activity;
            if (!(ann != null && activity != null))
                throw new ChalkableException("Invalid param type");

            ann.Content = activity.Description;
            ann.MaxScore = activity.MaxScore;
            ann.ClassRef = activity.SectionId;
            ann.Expires = activity.Date;
            ann.MayBeDropped = activity.MayBeDropped;
            ann.WeightAddition = activity.WeightAddition;
            ann.WeightMultiplier = activity.WeightMultiplier;
            ann.Dropped = activity.IsDropped;
            ann.ClassAnnouncementTypeRef = activity.CategoryId;
            ann.VisibleForStudent = activity.DisplayInHomePortal;
            ann.Title = activity.Name;

            if (ann is AnnouncementDetails && activity.Attachments != null && activity.Attachments.Any())
            {
                var annDetails = ann as AnnouncementDetails;
                if (annDetails.AnnouncementAttachments == null)
                    annDetails.AnnouncementAttachments = new List<AnnouncementAttachment>();
                foreach (var att in activity.Attachments)
                {
                    var annAtt = annDetails.AnnouncementAttachments.FirstOrDefault(x => x.SisAttachmentId == att.AttachmentId);
                    if (annAtt == null)
                    {
                        annAtt = new AnnouncementAttachment
                        {
                            AnnouncementRef = annDetails.Id,
                            SisAttachmentId = att.AttachmentId
                        };
                        annDetails.AnnouncementAttachments.Add(annAtt);
                    }
                    ModelMapper.GetMapper().Map(annAtt, att);
                    annAtt.Name = att.Name;
                    annAtt.Uuid = att.CrocoDocId.ToString();
                }
            }
        }
    }

    public class AnnouncementComplexToActivity : IMapper
    {
        public void Map(object returnObj, object sourceObj)
        {
            var ann = sourceObj as AnnouncementComplex;
            var activity = returnObj  as Activity;
            if (!(ann != null && activity != null))
                throw new ChalkableException("Invalid param type");

            activity.Date = ann.Expires;
            activity.CategoryId = ann.ClassAnnouncementTypeRef;
            activity.IsDropped = ann.Dropped;
            activity.IsScored = ann.ClassAnnouncementTypeRef.HasValue;
            activity.MaxScore = ann.MaxScore;
            activity.WeightAddition = ann.WeightAddition;
            activity.WeightMultiplier = ann.WeightMultiplier;
            activity.MayBeDropped = ann.MayBeDropped;
            activity.Description = ann.Content;
            activity.DisplayInHomePortal = ann.VisibleForStudent;
            activity.Name = ann.Title;
            activity.Unit = string.Empty;
            if (ann.ClassRef.HasValue)
                activity.SectionId = ann.ClassRef.Value;

            var annDetails = ann as AnnouncementDetails;
            if (annDetails != null && annDetails.AnnouncementAttachments != null && annDetails.AnnouncementAttachments.Count > 0)
            {
                if (activity.Attachments == null)
                    activity.Attachments = new List<ActivityAttachment>();
                var newAtts = new List<ActivityAttachment>();
                foreach (var annAtt in annDetails.AnnouncementAttachments)
                {
                    if (annAtt.SisAttachmentId.HasValue)
                    {
                        var att = activity.Attachments.FirstOrDefault(x => x.AttachmentId == annAtt.SisAttachmentId);
                        if (att == null)
                        {
                            att = new ActivityAttachment { ActivityId = activity.Id };
                            newAtts.Add(att);
                        }
                        ModelMapper.GetMapper().Map(att, annAtt);
                    }
                }
                activity.Attachments = activity.Attachments.Concat(newAtts);
            }
        }
    }

    public class AnnouncementAttToActivityAttMapper : IMapper
    {
        public void Map(object returnObj, object sourceObj)
        {
            var activityAtt = returnObj as ActivityAttachment;
            var announcementAtt = sourceObj as AnnouncementAttachment;
            if (!(activityAtt != null && announcementAtt != null))
                throw new ChalkableException("Invalid param type");

            if(announcementAtt.SisAttachmentId.HasValue)
                activityAtt.AttachmentId = announcementAtt.SisAttachmentId.Value;
            activityAtt.CrocoDocId = new Guid(announcementAtt.Uuid);
            activityAtt.Name = announcementAtt.Name;
        }
    }

    public class StiAttachmentToAnnouncementAttMapper: IMapper
    {
        public void Map(object returnObj, object sourceObj)
        {
            var activityAtt = sourceObj as StiAttachment;
            var announcementAtt = returnObj  as AnnouncementAttachment;
            if (!(activityAtt != null && announcementAtt != null))
                throw new ChalkableException("Invalid param type");

            announcementAtt.Name = activityAtt.Name;
            announcementAtt.SisAttachmentId = activityAtt.AttachmentId;
            if (activityAtt.CrocoDocId.HasValue)
                announcementAtt.Uuid = activityAtt.CrocoDocId.Value.ToString();

            if (sourceObj is ActivityAttachment)
                announcementAtt.SisActivityId = (sourceObj as ActivityAttachment).ActivityId;
        }
    }

    public class ModelMapper : IModelMapper
    {
        private static IDictionary<Pair<Type,Type>, IMapper> _customMappers 
            = new Dictionary<Pair<Type, Type>, IMapper>();

        private static ModelMapper _modelMapper;
        private  ModelMapper()
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
                    {new Pair<Type, Type>(typeof(Activity), typeof(AnnouncementComplex)), new AnnouncementComplexToActivity()},
                    {new Pair<Type, Type>(typeof(Activity), typeof(AnnouncementDetails)), new AnnouncementComplexToActivity()},
                    {new Pair<Type, Type>(typeof(ActivityAttachment), typeof(AnnouncementAttachment)), new AnnouncementAttToActivityAttMapper()},
                    {new Pair<Type, Type>(typeof(AnnouncementAttachment), typeof(ActivityAttachment)), new StiAttachmentToAnnouncementAttMapper()},
                    {new Pair<Type, Type>(typeof(AnnouncementAttachment), typeof(StiAttachment)), new StiAttachmentToAnnouncementAttMapper()},
               
                };
        }

        public static ModelMapper GetMapper()
        {
            return _modelMapper ?? (_modelMapper = new ModelMapper());
        }

        public void Map<TReturn, TSource>(TReturn returnObj, TSource sourceObj) where TReturn : class where TSource : class
        {
            var typesObj = new Pair<Type, Type>(returnObj.GetType(), sourceObj.GetType());
            if (!_customMappers.ContainsKey(typesObj))
                throw new ChalkableException("There are no mapper with such source and return types");

            _customMappers[typesObj].Map(returnObj, sourceObj);
        }
    }
}
