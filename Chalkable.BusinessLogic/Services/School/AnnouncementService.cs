using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Chalkable.BusinessLogic.Mapping.ModelMappers;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.DataAccess.AnnouncementsDataAccess;
using Chalkable.Data.School.Model;
using Chalkable.Data.School.Model.Announcements;
using Chalkable.StiConnector.Connectors.Model;

namespace Chalkable.BusinessLogic.Services.School
{

    //public interface IAnnouncementService
    //{
    //    AnnouncementDetails CreateAnnouncement(ClassAnnouncementType classAnnType, int classId, DateTime expiresDate);
    //    AnnouncementDetails CreateAdminAnnouncement(DateTime expiresDate);
    //    AnnouncementDetails GetAnnouncementDetails(int announcementId);
    //    void DeleteAnnouncement(int announcementId);
    //    void DeleteAnnouncements(int classId, int? announcementType, AnnouncementState state);
    //    void DeleteAnnouncements(int schoolpersonid, AnnouncementState state = AnnouncementState.Draft);

    //    Announcement EditTitle(int announcementId, string title);
    //    bool Exists(string title, int classId, DateTime expiresDate, int? excludeAnnouncementId);

    //    AnnouncementDetails EditAnnouncement(ClassAnnouncementInfo announcement, int? classId = null, IList<int> groupsIds = null);
    //    void SubmitAnnouncement(int announcementId, int recipientId);
    //    void SubmitForAdmin(int announcementId);

    //    Announcement GetAnnouncementById(int id);
    //    IList<AnnouncementComplex> GetAdminAnnouncements(bool? complete, IList<int> gradeLevels, DateTime? fromDate, DateTime? toDate, int? start, int? count, bool ownerOnly = false, int? studentId = null); 
    //    IList<AnnouncementComplex> GetAnnouncementsForFeed(bool? complete, int start, int count, int? classId, int? markingPeriodId = null, bool ownerOnly = false, bool? graded = null);
    //    IList<AnnouncementComplex> GetAnnouncements(DateTime? fromDate, DateTime? toDate, bool onlyOwners = false, int? classId = null, bool includeAdminAnnouncement = false);
    //    IList<AnnouncementComplex> GetAnnouncements(string filter);
    //    IList<AnnouncementComplex> GetAnnouncementsComplex(AnnouncementsQuery query, IList<Activity> activities = null);
    //    Announcement GetLastDraft();
    //    Announcement DropUnDropAnnouncement(int announcementId, bool drop);
    //    IList<Announcement> GetDroppedAnnouncement(int markingPeriodClassId);
    //    IList<Person> GetAnnouncementRecipientPersons(int announcementId); 
    //    int GetNewAnnouncementItemOrder(AnnouncementDetails announcement);
    //    void SetComplete(int id, bool complete);
    //    void SetAnnouncementsAsComplete(DateTime? date, bool complete);
    //    Announcement SetVisibleForStudent(int id, bool visible);
    //    IList<string> GetLastFieldValues(int personId, int classId, int classAnnouncementType);
    //    bool CanAddStandard(int announcementId);
    //    Standard AddAnnouncementStandard(int announcementId, int standardId);
    //    Standard RemoveStandard(int announcementId, int standardId);
    //    void RemoveAllAnnouncementStandards(int standardId);
    //    IList<AnnouncementStandard> GetAnnouncementStandards(int classId);
    //    void CopyAnnouncement(int id, IList<int> classIds);

    //    void SubmitGroupsToAnnouncement(int announcementId, IList<int> groupsIds);
    //}

    //public class AnnouncementService : SisConnectedService, IAnnouncementService
    //{
    //    public AnnouncementService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
    //    {
    //    }

    //    private AnnouncementDataAccess CreateAnnoucnementDataAccess(UnitOfWork unitOfWork)
    //    {
    //        Trace.Assert(Context.SchoolLocalId.HasValue);
    //        if(Context.Role == CoreRoles.TEACHER_ROLE)
    //            return new AnnouncementForTeacherDataAccess(unitOfWork, Context.SchoolLocalId.Value);
    //        if(Context.Role == CoreRoles.STUDENT_ROLE)
    //            return new AnnouncementForStudentDataAccess(unitOfWork, Context.SchoolLocalId.Value);
    //        if(Context.Role == CoreRoles.DISTRICT_ADMIN_ROLE)
    //            return new AdminAnnouncementDataAccess(unitOfWork);
    //        throw new ChalkableException("Unsupported role for announcements");
    //    }


    //    public AnnouncementQueryResult GetAnnouncements(AnnouncementsQuery query)
    //    {
    //        using (var uow = Read())
    //        {
    //            var da = CreateAnnoucnementDataAccess(uow);
    //            query.RoleId = Context.Role.Id;
    //            query.PersonId = Context.PersonId;
    //            query.Now = Context.NowSchoolYearTime.Date;
    //            var res = da.GetAnnouncements(query);
    //            return res;
    //        }
    //    }

    //    public IList<AnnouncementComplex> GetAdminAnnouncements(bool? complete, IList<int> gradeLevels, DateTime? fromDate, DateTime? toDate, int? start, int? count,
    //                                       bool ownedOnly = false, int? studentId = null)
    //    {
    //        return GetAnnouncements(new AnnouncementsQuery
    //            {
    //                GradeLevelsIds = gradeLevels,
    //                FromDate = fromDate,
    //                ToDate = toDate,
    //                Start = start ?? 0,
    //                Count = count ?? int.MaxValue,
    //                OwnedOnly = ownedOnly, 
    //                Complete = complete,
    //                StudentId = studentId
    //            }).Announcements;
    //    }

    //    public IList<AnnouncementComplex> GetAnnouncementsForFeed(bool? complete, int start, int count, int? classId, int? markingPeriodId = null, bool ownerOnly = false, bool? graded = null)
    //    {
    //        var isStudent = Context.Role == CoreRoles.STUDENT_ROLE;
    //        var q = new AnnouncementsQuery
    //        {
    //            Complete = complete,
    //            Start = start,
    //            Count = isStudent ? count + 1 : count,
    //            ClassId = classId,
    //            MarkingPeriodId = markingPeriodId,
    //            OwnedOnly = ownerOnly,
    //            Graded = graded
    //        };
    //        var res = GetAnnouncementsComplex(q);
    //        if (isStudent)
    //        {
    //            var fromDate = q.Start != 0 && res.Count > 0 ? res.Min(x => x.Expires) : DateTime.MinValue;
    //            var toDate = res.Count > 0 ? res.Max(x => x.Expires) : DateTime.MaxValue;
    //            if(res.Count == q.Count)
    //                res.RemoveAt(res.Count - 1);

    //            var adminAnnouncements = GetAnnouncements(new AnnouncementsQuery
    //                {
    //                    FromDate = fromDate, 
    //                    ToDate = toDate, 
    //                    Complete = complete, 
    //                    AdminOnly = true
    //                }).Announcements;
    //            res = res.Concat(adminAnnouncements).OrderBy(x => x.Expires).ToList();
    //        }
    //        return res;
    //    }

    //    public IList<AnnouncementComplex> GetAnnouncements(DateTime? fromDate, DateTime? toDate, bool onlyOwners = false, int? classId = null, bool includeAdminAnnouncement = false)
    //    {
    //        var res = new List<AnnouncementComplex>();
    //        if (!BaseSecurity.IsDistrictAdmin(Context))
    //        {
    //            var q = new AnnouncementsQuery
    //            {
    //                FromDate = fromDate.HasValue ? fromDate.Value.Date : (DateTime?)null,
    //                ToDate = toDate.HasValue ? toDate.Value.Date : (DateTime?)null,
    //                ClassId = classId
    //            };
    //            res.AddRange(GetAnnouncementsComplex(q));
    //        }
    //        if (Context.Role == CoreRoles.STUDENT_ROLE && includeAdminAnnouncement && !classId.HasValue)
    //        {
    //            var adminAnnouncements = GetAnnouncements(new AnnouncementsQuery { FromDate = fromDate, ToDate = toDate, AdminOnly = true }).Announcements;
    //            res = res.Concat(adminAnnouncements).OrderBy(x => x.Expires).ToList();
    //        }
    //        return res;
    //    }

    //    public IList<AnnouncementComplex> GetAnnouncements(string filter)
    //    {
    //        //TODO : rewrite impl for better performance
    //        var anns = GetAnnouncements(new AnnouncementsQuery{OwnedOnly = true}).Announcements;
    //        if (!BaseSecurity.IsDistrictAdmin(Context))
    //        {
    //            if (Context.Role == CoreRoles.STUDENT_ROLE)
    //            {
    //                var classPersons = ServiceLocator.ClassService.GetClassPersons(Context.PersonId.Value, true);
    //                var sy = ServiceLocator.SchoolYearService.GetCurrentSchoolYear();
    //                var markingPeriods = ServiceLocator.MarkingPeriodService.GetMarkingPeriods(sy.Id);
    //                classPersons = classPersons.Where(x => markingPeriods.Any(y => y.Id == x.MarkingPeriodRef)).ToList();
    //                anns = anns.Where(x => classPersons.Any(cp => cp.ClassRef == x.ClassRef)).ToList();
    //            }
    //            var classesIds = anns.Where(x => x.ClassRef.HasValue).GroupBy(x => x.ClassRef.Value).Select(x => x.Key).ToList();
    //            var classAnnTypes = ServiceLocator.ClassAnnouncementTypeService.GetClassAnnouncementTypes(classesIds);
    //            foreach (var ann in anns)
    //            {
    //                if (!ann.ClassAnnouncementTypeRef.HasValue) continue;
    //                var classAnnType = classAnnTypes.FirstOrDefault(x => x.Id == ann.ClassAnnouncementTypeRef);
    //                if (classAnnType == null) continue;
    //                ann.ClassAnnouncementTypeName = classAnnType.Name;
    //                ann.ChalkableAnnouncementType = classAnnType.ChalkableAnnouncementTypeRef;
    //            }
    //        }
    //        IList<AnnouncementComplex> res = new List<AnnouncementComplex>();
    //        if (!string.IsNullOrEmpty(filter))
    //        {
    //            string[] words = filter.Split(new[] { ' ', '.', ',' }, StringSplitOptions.RemoveEmptyEntries);
    //            for (int i = 0; i < words.Count(); i++)
    //            {
    //                string word = words[i];
    //                int annOrder;
    //                IList<AnnouncementComplex> curretnAnns = new List<AnnouncementComplex>();
    //                if (int.TryParse(words[i], out annOrder))
    //                {
    //                    curretnAnns = anns.Where(x => x.Order == annOrder).ToList();
    //                }
    //                else
    //                {
    //                    curretnAnns = anns.Where(x =>
    //                                     ( x.Subject != null && x.Subject.ToLower().Contains(word))
    //                                     || (!(string.IsNullOrEmpty(x.ClassName)) && (x.ClassName.ToLower().Contains(word))) 
    //                                     || ("all".Contains(word))
    //                                     || (!string.IsNullOrEmpty(x.ClassAnnouncementTypeName) &&  x.ClassAnnouncementTypeName.ToLower().Contains(word))
    //                                     || x.Title != null && x.Title.ToLower().Contains(word)
    //                                     || x.Content != null && x.Content.ToLower().Contains(word)
    //                                     ).ToList();
    //                }
    //                res = res.Union(curretnAnns).ToList();
    //            }
    //        }
    //        return res;
    //    }

        


    //    public void DeleteAnnouncement (int announcementId)
    //    {
    //        if (!Context.PersonId.HasValue)
    //            throw new UnassignedUserException();
    //        using (var uow = Update())
    //        {
    //            var da = CreateAnnoucnementDataAccess(uow);
    //            var announcement = da.GetAnnouncement(announcementId, Context.PersonId.Value);
    //            if (!AnnouncementSecurity.CanDeleteAnnouncement(announcement, Context))
    //                throw new ChalkableSecurityException();

    //            if(announcement.SisActivityId.HasValue)
    //                ConnectorLocator.ActivityConnector.DeleteActivity(announcement.SisActivityId.Value);
    //            da.Delete(announcementId, null, null, null, null);
    //            uow.Commit();
    //        }
    //    }

    //    public void DeleteAnnouncements(int classId, int? announcementType, AnnouncementState state)
    //    {
    //        DoUpdate(u => CreateAnnoucnementDataAccess(u).Delete(null, Context.PersonId, classId, announcementType, state));
    //    }

    //    public void DeleteAnnouncements(int personId, AnnouncementState state = AnnouncementState.Draft)
    //    {
    //        if(!AnnouncementSecurity.CanDeleteAnnouncement(personId, Context))
    //            throw new ChalkableSecurityException();
    //        DoUpdate(u=> CreateAnnoucnementDataAccess(u).Delete(null, Context.PersonId, null, null, state));
    //    }

    //    public IList<Announcement> GetDroppedAnnouncement(int markingPeriodClassId)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    private void RecreateAdminAnnouncementRecipients(int announcementId, IEnumerable<int> groupsIds, UnitOfWork uow)
    //    {
    //        if (groupsIds == null) return;
    //        var da = new DataAccessBase<AnnouncementGroup, int>(uow);
    //        var annsRecipients = da.GetAll(new AndQueryCondition {{AnnouncementGroup.ANNOUNCEMENT_REF_FIELD, announcementId}});
    //        da.Delete(annsRecipients);
    //        groupsIds = groupsIds.Distinct();
    //        var annRecipients = groupsIds.Select(gId => new AnnouncementGroup
    //            {
    //                AnnouncementRef = announcementId,
    //                GroupRef = gId
    //            }).ToList();
    //        da.Insert(annRecipients);
    //    }


    //    private Announcement Submit(AnnouncementDataAccess dataAccess, UnitOfWork unitOfWork, int announcementId, int classId)
    //    {

    //        var res = GetAnnouncementDetails(announcementId);
    //        AnnouncementSecurity.EnsureInModifyAccess(res, Context);
    //        SetClassToAnnouncement(res, classId);
    //        if (res.State == AnnouncementState.Draft)
    //        {
    //            res.State = AnnouncementState.Created;
    //            res.Created = Context.NowSchoolTime.Date;
    //            if (string.IsNullOrEmpty(res.Title) || res.DefaultTitle == res.Title)
    //                res.Title = res.DefaultTitle;

    //            var activity = new Activity();
    //            MapperFactory.GetMapper<Activity, AnnouncementDetails>().Map(activity, res);
    //            activity = ConnectorLocator.ActivityConnector.CreateActivity(classId, activity);
    //            if(CreateAnnoucnementDataAccess(unitOfWork).Exists(activity.Id))
    //                throw new ChalkableException("Announcement with such activityId already exists");
    //            res.SisActivityId = activity.Id;      
    //        }
    //        dataAccess.Update(res);
    //        return res;
    //    }

    //    public void SubmitAnnouncement(int announcementId, int recipientId)
    //    {
    //        Trace.Assert(Context.SchoolLocalId.HasValue);
    //        using (var uow = Update())
    //        {
    //            var da = CreateAnnoucnementDataAccess(uow);
    //            var res = Submit(da, uow, announcementId, recipientId);

    //            var sy = new SchoolYearDataAccess(uow).GetByDate(res.Expires, Context.SchoolLocalId.Value);
    //            if(res.ClassAnnouncementTypeRef.HasValue)
    //                da.ReorderAnnouncements(sy.Id, res.ClassAnnouncementTypeRef.Value, recipientId);
    //            uow.Commit();
    //        }
    //    }
        
    //    public void SubmitForAdmin(int announcementId)
    //    {
    //        BaseSecurity.EnsureDistrictAdmin(Context);
    //        using (var uow = Update())
    //        {
    //            var da = CreateAnnoucnementDataAccess(uow);
    //            var annRecipients = new DataAccessBase<AnnouncementGroup>(uow)
    //                .GetAll(new AndQueryCondition{{AnnouncementGroup.ANNOUNCEMENT_REF_FIELD, announcementId}});
    //            if(annRecipients.Count == 0)
    //                throw new ChalkableException("Admin Announcement has no groups. You can't sumbit admin announcement without selected groups");
    //            var ann = da.GetDetails(announcementId, Context.PersonId.Value, Context.RoleId);
    //            AnnouncementSecurity.EnsureInModifyAccess(ann, Context);
    //            if (string.IsNullOrEmpty(ann.Title))
    //                throw new ChalkableException(string.Format(ChlkResources.ERR_PARAM_IS_MISSING_TMP, "Announcement Title "));
    //            if (ann.State == AnnouncementState.Draft)
    //            {
    //                ann.Created = Context.NowSchoolTime;
    //                ann.State = AnnouncementState.Created;
    //            }
    //            da.Update(ann);
    //            uow.Commit();
    //        }
    //    }
        
    //    private void SetClassToAnnouncement(Announcement announcement, int? classId)
    //    {
    //        if (classId.HasValue)
    //        {
    //            if (announcement.State == AnnouncementState.Draft)
    //            {
    //                announcement.ClassRef = classId.Value;
    //                return;
    //            }

    //            // Fetch current or prev marking period, 'cause if this method is called even when expiresDate wasn't supplied by teacher
    //            /*var mp = ServiceLocator.MarkingPeriodService.GetMarkingPeriodByDate(expiresDate.Date, true);
    //            if (mp == null)
    //                throw new NoMarkingPeriodException();
    //            var mpc = ServiceLocator.MarkingPeriodService.GetMarkingPeriodClass(classId.Value, mp.Id);
    //            if (mpc == null)
    //                throw new ChalkableException(ChlkResources.ERR_CLASS_IS_NOT_SCHEDULED_FOR_MARKING_PERIOD);*/
    //            if (announcement.State == AnnouncementState.Created && announcement.ClassRef != classId)
    //                throw new ChalkableException("Class can't be changed for submmited announcement");
    //            announcement.ClassRef = classId.Value;   
    //        }
            
    //    }
       
    //    public Announcement GetAnnouncementById(int id)
    //    {
    //        if(!Context.PersonId.HasValue)
    //            throw new UnassignedUserException();

    //        using (var uow = Read())
    //        {
    //            var res = CreateAnnoucnementDataAccess(uow).GetAnnouncement(id, Context.PersonId.Value); // security here 
    //            if(res == null)
    //                throw new NoAnnouncementException();
    //            return res;
    //        }
    //    }

    //    public int GetNewAnnouncementItemOrder(AnnouncementDetails announcement)
    //    {
    //        var attOrder = announcement.AnnouncementAttachments.Max(x => (int?)x.Order);
    //        var appOrder = announcement.AnnouncementApplications.Max(x => (int?)x.Order);
    //        var order = 0;
    //        if (attOrder.HasValue)
    //        {
    //            if (appOrder.HasValue)
    //            {
    //                order = Math.Max(attOrder.Value, appOrder.Value) + 1;
    //            }
    //            else
    //            {
    //                order = attOrder.Value + 1;
    //            }
    //        }
    //        else
    //        {
    //            if (appOrder.HasValue)
    //            {
    //                order = appOrder.Value + 1;
    //            }
    //        }
    //        return order;
    //    }

    //    public void SetComplete(int id, bool complete)
    //    {
    //        if (!Context.PersonId.HasValue)
    //            throw new UnassignedUserException();
    //        var ann = GetAnnouncementById(id);
    //        if (!ann.IsDraft)
    //            throw new ChalkableException("Not created item can't be starred");
            
    //        if (ann.IsAdminAnnouncement)
    //        {
    //            DoUpdate(u=> new AnnouncementRecipientDataDataAccess(u)
    //                .UpdateAdminAnnouncementData(ann.Id, Context.PersonId.Value, complete));
    //            return;
    //        }
            
    //        if (!ann.SisActivityId.HasValue)
    //            throw new ChalkableException("There are no such item in Inow");
    //        ConnectorLocator.ActivityConnector.CompleteActivity(ann.SisActivityId.Value, complete);
    //    }

    //    public Announcement SetVisibleForStudent(int id, bool visible)
    //    {
    //        var ann = GetAnnouncementById(id);
    //        if (ann.SisActivityId.HasValue)
    //        {
    //            var activity = ConnectorLocator.ActivityConnector.GetActivity(ann.SisActivityId.Value);
    //            activity.DisplayInHomePortal = visible;
    //            ConnectorLocator.ActivityConnector.UpdateActivity(ann.SisActivityId.Value, activity);
                   
    //            using (var uow = Update())
    //            {
    //                ann.VisibleForStudent = visible;
    //                CreateAnnoucnementDataAccess(uow).Update(ann);
    //                uow.Commit();
    //            }
    //        }
    //        return ann;
    //    }

    //    public Announcement GetLastDraft()
    //    {
    //        if(!Context.PersonId.HasValue)
    //            throw new UnassignedUserException();
    //        return DoRead(u => CreateAnnoucnementDataAccess(u).GetLastDraft(Context.PersonId.Value));
    //    }

    //    public IList<Person> GetAnnouncementRecipientPersons(int announcementId)
    //    {
    //        var ann = GetAnnouncementById(announcementId);
    //        if (ann.State == AnnouncementState.Draft)
    //            throw new ChalkableException(ChlkResources.ERR_NO_RECIPIENTS_IN_DRAFT_STATE);
    //        return DoRead( u => CreateAnnoucnementDataAccess(u).GetAnnouncementRecipientPersons(announcementId, Context.PersonId ?? 0));
    //    }

    //    public IList<string> GetLastFieldValues(int personId, int classId, int classAnnouncementType)
    //    {
    //        return DoRead(u => CreateAnnoucnementDataAccess(u).GetLastFieldValues(personId, classId, classAnnouncementType, 10));
    //    }

    //    public Announcement EditTitle(int announcementId, string title)
    //    {
    //        var ann = GetAnnouncementById(announcementId);
    //        if(!ann.ClassRef.HasValue)
    //            throw new NotImplementedException(); //TODO: implement for non class announcement
    //        return EditTitle(ann, title, (da, t) => da.Exists(t, ann.ClassRef.Value, ann.Expires, announcementId));
    //    }

    //    private Announcement EditTitle(Announcement announcement, string title, Func<AnnouncementDataAccess, string, bool> existsTitleAction)
    //    {
    //        if (!Context.PersonId.HasValue)
    //            throw new UnassignedUserException();
    //        if (announcement != null)
    //        {
    //            if (announcement.Title != title)
    //            {
    //                using (var uow = Update())
    //                {
    //                    if (!announcement.IsOwner)
    //                        throw new ChalkableSecurityException();
    //                    var da = CreateAnnoucnementDataAccess(uow);
    //                    if (string.IsNullOrEmpty(title))
    //                        throw new ChalkableException("Title parameter is empty");
    //                    if (existsTitleAction(da, title))
    //                        throw new ChalkableException("The item with current title already exists");
    //                    announcement.Title = title;
    //                    da.Update(announcement);
    //                    uow.Commit();
    //                }
    //            }    
    //        }
    //        return announcement;
    //    }

    //    public bool Exists(string title, int classId, DateTime expiresDate, int? announcementId)
    //    {
    //        return DoRead(u => CreateAnnoucnementDataAccess(u).Exists(title, classId, expiresDate, announcementId));
    //    }

    //    public Standard AddAnnouncementStandard(int announcementId, int standardId)
    //    {
    //        var ann = GetAnnouncementById(announcementId);
    //        AnnouncementSecurity.EnsureInModifyAccess(ann, Context);
    //        using (var uow = Update())
    //        {
    //            new AnnouncementStandardDataAccess(uow)
    //                .Insert(new AnnouncementStandard
    //                    {
    //                        AnnouncementRef = announcementId,
    //                        StandardRef = standardId
    //                    });
    //            if (ann.State == AnnouncementState.Created && ann.SisActivityId.HasValue)
    //            {
    //                var activity = ConnectorLocator.ActivityConnector.GetActivity(ann.SisActivityId.Value);
    //                activity.Standards = activity.Standards.Concat(new [] {new ActivityStandard {Id = standardId}});
    //                ConnectorLocator.ActivityConnector.UpdateActivity(ann.SisActivityId.Value, activity);
    //            }
    //            uow.Commit();
    //            return new StandardDataAccess(uow).GetById(standardId);
    //        }
    //    }
        
    //    public Standard RemoveStandard(int announcementId, int standardId)
    //    {
    //        var ann = GetAnnouncementById(announcementId);
    //        if(!AnnouncementSecurity.CanModifyAnnouncement(ann, Context))
    //            throw new ChalkableSecurityException();
    //        using (var uow = Update())
    //        {
    //            new AnnouncementStandardDataAccess(uow).Delete(announcementId, standardId);
    //            if (ann.State == AnnouncementState.Created && ann.SisActivityId.HasValue)
    //            {
    //                var activity = ConnectorLocator.ActivityConnector.GetActivity(ann.SisActivityId.Value);
    //                activity.Standards = activity.Standards.Where(x => x.Id != standardId).ToList();
    //                ConnectorLocator.ActivityConnector.UpdateActivity(ann.SisActivityId.Value, activity);
    //            }
    //            uow.Commit();
    //            return new StandardDataAccess(uow).GetById(standardId);
    //        }
    //    }

    //    public void RemoveAllAnnouncementStandards(int standardId)
    //    {
    //        BaseSecurity.EnsureSysAdmin(Context);
    //        DoUpdate(u => new AnnouncementStandardDataAccess(u).DeleteAll(standardId));
    //    }

    //    public bool CanAddStandard(int announcementId)
    //    {
    //        return DoRead(u => CreateAnnoucnementDataAccess(u).CanAddStandard(announcementId));
    //    }

    //    public IList<AnnouncementStandard> GetAnnouncementStandards(int classId)
    //    {
    //        return DoRead(u => new AnnouncementStandardDataAccess(u).GetAnnouncementStandardsByClassId(classId));
    //    }
        
    //    public void CopyAnnouncement(int id, IList<int> classIds)
    //    {
    //        var ann = GetAnnouncementById(id);
    //        if(ann.State != AnnouncementState.Created)
    //            throw new ChalkableException("Current announcement is not submited yet");
    //        if(!ann.SisActivityId.HasValue)
    //            throw new ChalkableException("Current announcement doesn't have activityId");
    //        ConnectorLocator.ActivityConnector.CopyActivity(ann.SisActivityId.Value, classIds);
    //    }

    //    public void SetAnnouncementsAsComplete(DateTime? toDate, bool complete)
    //    {
    //        if(!Context.PersonId.HasValue)
    //            throw new UnassignedUserException();
    //        var syId = Context.SchoolYearId ?? ServiceLocator.SchoolYearService.GetCurrentSchoolYear().Id;
    //        if(BaseSecurity.IsDistrictAdmin(Context))
    //            CompleteAdminAnnouncement(Context.PersonId.Value, complete, toDate);
    //        if(CoreRoles.TEACHER_ROLE == Context.Role)
    //            ConnectorLocator.ActivityConnector.CompleteTeacherActivities(syId, Context.PersonId.Value, complete, toDate);
    //        if(CoreRoles.STUDENT_ROLE == Context.Role)
    //            ConnectorLocator.ActivityConnector.CompleteStudentActivities(syId, Context.PersonId.Value, complete, toDate);
    //    }

    //    private void CompleteAdminAnnouncement(int personId, bool complete, DateTime? toDate)
    //    {
    //        DoUpdate(u =>
    //            {
    //                var anns = CreateAnnoucnementDataAccess(u)
    //                    .GetAnnouncements(new AnnouncementsQuery {PersonId = personId, ToDate = toDate})
    //                    .Announcements;
    //                var da = new AnnouncementRecipientDataDataAccess(u);
    //                foreach (var ann in anns)
    //                    da.UpdateAnnouncementRecipientData(ann.Id, personId, complete);
    //            });
    //    }

    //    public void SubmitGroupsToAnnouncement(int announcementId, IList<int> groupsIds)
    //    {
    //        if (!Context.PersonId.HasValue)
    //            throw new UnassignedUserException();
    //        var ann = GetAnnouncementById(announcementId); //security check
    //        DoUpdate(u => RecreateAdminAnnouncementRecipients(ann.Id, groupsIds, u));
    //    }
    //}
}
