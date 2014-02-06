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

    public abstract class BaseMapper<TReturn, TSource> : IMapper 
        where TReturn : class 
        where TSource : class 
    {
        public void Map(object returnObj, object sourceObj)
        {
            var obj1 = returnObj as TReturn;
            var obj2 = sourceObj as TSource;
            if(obj1 == null || obj2 == null)
                throw new ChalkableException("Invalid param type");
            InnerMap(obj1, obj2);
        }
        protected abstract void InnerMap(TReturn returnObj, TSource sourceObj);
    }

    public class ActivityToAnnouncementMapper : BaseMapper<Announcement, Activity>
    {
        protected override void InnerMap(Announcement ann, Activity activity)
        {
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
                    MapperFactory.GetMapper<AnnouncementAttachment, ActivityAttachment>().Map(annAtt, att);
                }

                if (activity.Standards != null && activity.Standards.Any())
                {
                    if (annDetails.AnnouncementStandards == null)
                        annDetails.AnnouncementStandards = new List<AnnouncementStandardDetails>();
                    foreach (var activityStandard in activity.Standards)
                    {
                        var annStandard = annDetails.AnnouncementStandards.FirstOrDefault(x => x.StandardRef == activityStandard.StandardId);
                        if (annStandard == null)
                        {
                            annStandard = new AnnouncementStandardDetails() {AnnouncementRef = annDetails.Id};
                            annDetails.AnnouncementStandards.Add(annStandard);
                        }
                        annStandard.StandardRef = activityStandard.StandardId;
                        if(annStandard.Standard == null)
                            annStandard.Standard = new Standard();
                        annStandard.Standard.Id = activityStandard.StandardId;
                        annStandard.Standard.Name = activityStandard.StandardName;
                    }                    
                }
            }
        }
    }

    public class AnnouncementComplexToActivityMapper : BaseMapper<Activity, AnnouncementComplex>
    {
        protected override void InnerMap(Activity activity, AnnouncementComplex ann)
        {
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
            if (annDetails != null)
            {
                if (annDetails.AnnouncementAttachments != null && annDetails.AnnouncementAttachments.Count > 0)
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
                            MapperFactory.GetMapper<ActivityAttachment, AnnouncementAttachment>().Map(att, annAtt);
                        }
                    }
                    activity.Attachments = activity.Attachments.Concat(newAtts);
                }
                if (annDetails.AnnouncementStandards != null && annDetails.AnnouncementStandards.Count > 0)
                {
                    if(activity.Standards == null)
                        activity.Standards = new LinkedList<ActivityStandard>();
                    var newStandards = new List<ActivityStandard>();
                    foreach (var annStandard in annDetails.AnnouncementStandards)
                    {
                        if (annStandard.Standard != null)
                        {
                            var activitySt = activity.Standards.FirstOrDefault(x => x.StandardId == annStandard.StandardRef);
                            if (activitySt == null)
                            {
                                activitySt = new ActivityStandard();
                                newStandards.Add(activitySt);
                            }
                            activitySt.StandardId = annStandard.Standard.Id;
                            activitySt.StandardName = annStandard.Standard.Name;
                        }
                    }
                    activity.Standards = activity.Standards.Concat(newStandards);
                }
            }
        }
    }

    public class AnnouncementAttToActivityAttMapper : BaseMapper<ActivityAttachment, AnnouncementAttachment>
    {
        protected override void InnerMap(ActivityAttachment activityAtt, AnnouncementAttachment announcementAtt)
        {
            if (announcementAtt.SisAttachmentId.HasValue)
                activityAtt.AttachmentId = announcementAtt.SisAttachmentId.Value;
            if(!string.IsNullOrEmpty(announcementAtt.Uuid))
                activityAtt.CrocoDocId = new Guid(announcementAtt.Uuid);
            activityAtt.Name = announcementAtt.Name;
        }
    }

    public class StiAttachmentToAnnouncementAttMapper: BaseMapper<AnnouncementAttachment, StiAttachment>
    {
        protected override void InnerMap(AnnouncementAttachment announcementAtt, StiAttachment activityAtt)
        {
            announcementAtt.Name = activityAtt.Name;
            announcementAtt.SisAttachmentId = activityAtt.AttachmentId;
            if (activityAtt.CrocoDocId.HasValue)
                announcementAtt.Uuid = activityAtt.CrocoDocId.Value.ToString();
            if (activityAtt is ActivityAttachment)
                announcementAtt.SisActivityId = (activityAtt as ActivityAttachment).ActivityId;
        }
    }

    public class MapperFactory 
    {
        private static IDictionary<Pair<Type,Type>, IMapper> _customMappers;

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
