using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.StiConnector.SyncModel;

namespace Chalkable.StiImport.Services
{
    public partial class ImportService
    {
        private void ProcessUpdate()
        {
            UpdateSchools();
            UpdateAddresses();
            UpdatePersons();
            UpdateSchoolPersons();
            UpdatePhones();
            UpdateGradeLevels();
            UpdateSchoolYears();
            UpdateStudentSchoolYears();
            UpdateMarkingPeriods();
            UpdateDayTypes();
            UpdateDays();
            UpdateRooms();
            UpdateCourses();
            UpdateStandards();
            UpdateMarkingPeriodClasses();
            UpdateClassAnnouncementTypes();
            UpdatePeriods();
            UpdateClassPeriods();
            UpdateClassPersons();
            UpdateAttendanceReasons();
            UpdateAlphaGrades();
            UpdateAlternateScores();
        }

        private void UpdateSchools()
        {
            //var schools = context.GetSyncResult<School>().Updated;
            
        }

        private void UpdateAddresses()
        {
            //var addresses = context.GetSyncResult<Address>().Updated;
            
        }

        private void UpdatePersons()
        {
            //throw new NotImplementedException();
        }

        private void UpdateSchoolPersons()
        {
            //throw new NotImplementedException();
        }

        private void UpdatePhones()
        {
            //throw new NotImplementedException();
        }

        private void UpdateGradeLevels()
        {
            //throw new NotImplementedException();
        }

        private void UpdateSchoolYears()
        {
            //throw new NotImplementedException();
        }

        private void UpdateStudentSchoolYears()
        {
            //throw new NotImplementedException();
        }

        private void UpdateMarkingPeriods()
        {
            //throw new NotImplementedException();
        }

        private void UpdateDayTypes()
        {
            //throw new NotImplementedException();
        }

        private void UpdateDays()
        {
            //throw new NotImplementedException();
        }

        private void UpdateRooms()
        {
            //throw new NotImplementedException();
        }

        private void UpdateCourses()
        {
            // throw new NotImplementedException();
        }

        private void UpdateStandards()
        {
            // throw new NotImplementedException();
        }

        private void UpdateMarkingPeriodClasses()
        {
            //throw new NotImplementedException();
        }

        private void UpdateClassAnnouncementTypes()
        {
            //throw new NotImplementedException();
        }

        private void UpdatePeriods()
        {
            //throw new NotImplementedException();
        }

        private void UpdateClassPeriods()
        {
            //throw new NotImplementedException();
        }

        private void UpdateClassPersons()
        {
            //throw new NotImplementedException();
        }

        private void UpdateAttendanceReasons()
        {
            //throw new NotImplementedException();
        }

        private void UpdateAlphaGrades()
        {
            // throw new NotImplementedException();
        }

        private void UpdateAlternateScores()
        {
            //throw new NotImplementedException();
        }
    }
}
