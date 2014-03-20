using System.Linq;
using Chalkable.BusinessLogic.Services.School;
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
            UpdateGradingPeriods();
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
            var schools = context.GetSyncResult<School>().Updated.Select(x=>new Data.School.Model.School
                {
                    Id = x.SchoolID,
                    IsActive = x.IsActive,
                    IsPrivate = x.IsPrivate,
                    Name = x.Name
                }).ToList();
            ServiceLocatorSchool.SchoolService.Edit(schools);
        }

        private void UpdateAddresses()
        {
            var addresses = context.GetSyncResult<Address>().Updated.Select(x=>new Data.School.Model.Address
                {
                    AddressLine1 = x.AddressLine1,
                    AddressLine2 = x.AddressLine2,
                    AddressNumber = x.AddressNumber,
                    City = x.City,
                    Country = x.Country,
                    CountyId = x.CountryID,
                    Id = x.AddressID,
                    Latitude = x.Latitude,
                    Longitude = x.Longitude,
                    PostalCode = x.PostalCode
                }).ToList();
            ServiceLocatorSchool.AddressService.Edit(addresses);
        }

        private void UpdatePersons()
        {
            var genders = context.GetSyncResult<Gender>().All.ToDictionary(x => x.GenderID);
            var persons = context.GetSyncResult<Person>().Updated.Select(x => new PersonInfo
                {
                    Id = x.PersonID,
                    Active = true,
                    AddressRef = x.PhysicalAddressID,
                    BirthDate = x.DateOfBirth,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Gender = x.GenderID.HasValue ? genders[x.GenderID.Value].Code : "U"
                }).ToList();
            ServiceLocatorSchool.PersonService.Edit(persons);
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

        private void UpdateGradingPeriods()
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
