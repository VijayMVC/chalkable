using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Master.Model;

namespace Chalkable.BusinessLogic.Logic
{
    public interface IStateMachine
    {
        bool CanApply(StateActionEnum action);
        void Apply(StateActionEnum action);
    }

    public class SchoolStateMachine : IStateMachine
    {
        private Guid schoolId;
        private IServiceLocatorMaster serviceLocatorMaster;

        private static  IDictionary<StateActionEnum, IDictionary<SchoolStatus, SchoolStatus>> stateMapper 
            = new Dictionary<StateActionEnum, IDictionary<SchoolStatus, SchoolStatus>>
                  {
                    {StateActionEnum.DataImport, new Dictionary<SchoolStatus, SchoolStatus>{{SchoolStatus.Created, SchoolStatus.DataImported}}},
                    {StateActionEnum.PersonalDataImport, new Dictionary<SchoolStatus, SchoolStatus>{{SchoolStatus.DataImported, SchoolStatus.PersonalInfoImported}}},
                    {StateActionEnum.MarkingPeriodsAdd, new Dictionary<SchoolStatus, SchoolStatus>{{SchoolStatus.PersonalInfoImported, SchoolStatus.MarkingPeriods}}},
                    {StateActionEnum.SectionsAdd, new Dictionary<SchoolStatus, SchoolStatus>{{SchoolStatus.MarkingPeriods, SchoolStatus.BlockScheduling}}},
                    {StateActionEnum.DailyPeriodsAdd, new Dictionary<SchoolStatus, SchoolStatus>{{SchoolStatus.BlockScheduling, SchoolStatus.DailyPeriods}}},
                    {StateActionEnum.ScheduleDataImport, new Dictionary<SchoolStatus, SchoolStatus>{{SchoolStatus.DailyPeriods, SchoolStatus.ScheduleInfoImported}}},

                    {StateActionEnum.InviteUser, new Dictionary<SchoolStatus, SchoolStatus>
                                                     {
                                                         {SchoolStatus.ScheduleInfoImported, SchoolStatus.InvitedUser}
                                                     }},
                    {StateActionEnum.InviteStudent, new Dictionary<SchoolStatus, SchoolStatus>
                                                    {
                                                        {SchoolStatus.TeacherLogged, SchoolStatus.InvitedStudent}
                                                    }},
                    {StateActionEnum.ActivateUser, new Dictionary<SchoolStatus, SchoolStatus>
                                                       {
                                                           {SchoolStatus.InvitedUser, SchoolStatus.TeacherLogged},
                                                       }},
                    {StateActionEnum.ActivateStudent, new Dictionary<SchoolStatus, SchoolStatus>
                                                       {
                                                           {SchoolStatus.InvitedStudent, SchoolStatus.StudentLogged}
                                                       }},
                    {StateActionEnum.SisImportAction, new Dictionary<SchoolStatus, SchoolStatus>
                                                       {
                                                            {SchoolStatus.Created, SchoolStatus.InvitedUser}        
                                                       }},
                  }; 

        private static IDictionary<StateActionEnum, SchoolStatus> actionStateMp = new Dictionary<StateActionEnum, SchoolStatus>
                                                                                          {
                                                                                              {StateActionEnum.PersonalDataImport, SchoolStatus.PersonalInfoImported},
                                                                                              {StateActionEnum.ScheduleDataImport, SchoolStatus.ScheduleInfoImported},
                                                                                          }; 


        public SchoolStateMachine(Guid schoolId, IServiceLocatorMaster serviceLocatorMaster)
        {
            this.schoolId = schoolId;
            this.serviceLocatorMaster = serviceLocatorMaster;
        }

        //TODO: in order to change import policy need to double check this logic
        public bool CanApply(StateActionEnum action)
        {
            return true;
            var school = serviceLocatorMaster.SchoolService.GetById(schoolId);
            return stateMapper.ContainsKey(action) &&  stateMapper[action].Any(x=>school.Status >= x.Key);
        }

        public void Apply(StateActionEnum action)
        {
            if(!CanApply(action))
                throw new InvalidSchoolStatusException();

            var school = serviceLocatorMaster.SchoolService.GetById(schoolId);
            SchoolStatus? status = null;
            if (stateMapper[action].ContainsKey(school.Status))
            {
                status = stateMapper[action][school.Status];
            }
            else if(actionStateMp.ContainsKey(action))
            {
                status = actionStateMp[action];
            }
            if (status.HasValue)
            {
                school.Status = status.Value;
                //TODO : thiink how to save school data 
            }
        }
    }

    public enum StateActionEnum
    {
       DataImport = 1,
       SisImportAction,
       PersonalDataImport,
       MarkingPeriodsAdd,
       SectionsAdd,
       DailyPeriodsAdd,
       ScheduleDataImport,
       InviteUser,
       ActivateUser,
       InviteStudent,
       ActivateStudent,
    }
}
