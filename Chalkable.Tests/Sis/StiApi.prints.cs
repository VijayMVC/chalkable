using System.Collections.Generic;
using System.Diagnostics;
using Chalkable.StiConnector.SyncModel;

namespace Chalkable.Tests.Sis
{

    public partial class StiApi : TestBase
    {
        private void Print(IEnumerable<Standard> items)
        {
            foreach (var item in items)
            {
                Debug.WriteLine($"{item.StandardID} {item.ParentStandardID} {item.StandardSubjectID} {item.SYS_CHANGE_VERSION} {item.SYS_CHANGE_CREATION_VERSION}");
            }
            Debug.WriteLine("---------------------------------");
        }

        private void Print(IEnumerable<Person> items)
        {
            foreach (var item in items)
            {
                Debug.WriteLine($"{item.PersonID}, {item.FirstName}, {item.LastName}, {item.DateOfBirth}, {item.GenderDescriptor}, {item.PhysicalAddressID}, {item.UserID} {item.SYS_CHANGE_VERSION} {item.SYS_CHANGE_CREATION_VERSION}");
            }
            Debug.WriteLine("---------------------------------");
        }

        private void Print(IEnumerable<StudentScheduleTerm> items)
        {
            foreach (var item in items)
            {
                Debug.WriteLine($"{item.StudentID} {item.SectionID} {item.TermID} {item.SYS_CHANGE_VERSION} {item.SYS_CHANGE_CREATION_VERSION}");
            }
            Debug.WriteLine("---------------------------------");
        }

        private void Print(IEnumerable<PersonTelephone> items)
        {
            foreach (var item in items)
            {
                Debug.WriteLine($"{item.PersonID} {item.TelephoneNumber} {item.SYS_CHANGE_VERSION} {item.SYS_CHANGE_CREATION_VERSION}");
            }
            Debug.WriteLine("---------------------------------");
        }

        private void Print(IEnumerable<Course> items)
        {
            foreach (var item in items)
            {
                Debug.WriteLine($"{item.CourseID} {item.CourseTypeID} {item.GradingScaleID} {item.SYS_CHANGE_VERSION} {item.SYS_CHANGE_CREATION_VERSION}");
            }
            Debug.WriteLine("---------------------------------");
        }

        private void Print(IEnumerable<User> items)
        {
            foreach (var item in items)
            {
                Debug.WriteLine($"{item.UserID} {item.SYS_CHANGE_VERSION} {item.SYS_CHANGE_CREATION_VERSION}");
            }
            Debug.WriteLine("---------------------------------");
        }

        private void Print(IEnumerable<UserSchool> items)
        {
            foreach (var item in items)
            {
                Debug.WriteLine($"{item.UserID} {item.SchoolID} {item.SYS_CHANGE_VERSION} {item.SYS_CHANGE_CREATION_VERSION}");
            }
            Debug.WriteLine("---------------------------------");
        }

        private void Print(IEnumerable<Room> items)
        {
            foreach (var item in items)
            {
                Debug.WriteLine($"{item.RoomID} {item.RoomNumber} {item.SYS_CHANGE_VERSION} {item.SYS_CHANGE_CREATION_VERSION}");
            }
            Debug.WriteLine("---------------------------------");
        }

        private void Print(IEnumerable<GradeLevel> items)
        {
            foreach (var item in items)
            {
                Debug.WriteLine($"{item.GradeLevelID} {item.Name} {item.Sequence} {item.SYS_CHANGE_VERSION} {item.SYS_CHANGE_CREATION_VERSION}");
            }
            Debug.WriteLine("---------------------------------");
        }

        private void Print(IEnumerable<StudentContact> items)
        {
            foreach (var item in items)
            {
                Debug.WriteLine($"{item.StudentID} {item.RelationshipID}  {item.ContactID} {item.SYS_CHANGE_VERSION} {item.SYS_CHANGE_CREATION_VERSION}");
            }
            Debug.WriteLine("---------------------------------");
        }

        private void Print(IEnumerable<ContactRelationship> items)
        {
            foreach (var item in items)
            {
                Debug.WriteLine($"{item.ContactRelationshipID} {item.Name} {item.SYS_CHANGE_VERSION} {item.SYS_CHANGE_CREATION_VERSION}");
            }
            Debug.WriteLine("---------------------------------");
        }

        private void Print(IEnumerable<GradingScale> items)
        {
            foreach (var item in items)
            {
                Debug.WriteLine($"{item.GradingScaleID} {item.Name} {item.SYS_CHANGE_VERSION} {item.SYS_CHANGE_CREATION_VERSION}");
            }
            Debug.WriteLine("---------------------------------");
        }

        private void Print(IEnumerable<Student> items)
        {
            foreach (var item in items)
            {
                Debug.WriteLine($"{item.StudentID} {item.UserID} {item.SYS_CHANGE_VERSION} {item.SYS_CHANGE_CREATION_VERSION}");
            }
            Debug.WriteLine("---------------------------------");
        }

        private void Print(IEnumerable<Address> items)
        {
            foreach (var item in items)
            {
                Debug.WriteLine($"{item.AddressID} {item.SYS_CHANGE_VERSION} {item.SYS_CHANGE_CREATION_VERSION}");
            }
            Debug.WriteLine("---------------------------------");
        }

        private void Print(IEnumerable<CourseType> items)
        {
            foreach (var item in items)
            {
                Debug.WriteLine($"{item.CourseTypeID} {item.SYS_CHANGE_VERSION} {item.SYS_CHANGE_CREATION_VERSION}");
            }
            Debug.WriteLine("---------------------------------");
        }

        private void Print(IEnumerable<CalendarDay> items)
        {
            foreach (var item in items)
            {
                Debug.WriteLine($"{item.AcadSessionID} {item.BellScheduleID} {item.DayTypeID} {item.SYS_CHANGE_VERSION} {item.SYS_CHANGE_CREATION_VERSION}");
            }
            Debug.WriteLine("---------------------------------");
        }

        private void Print(IEnumerable<BellSchedule> items)
        {
            foreach (var item in items)
            {
                Debug.WriteLine($"{item.AcadSessionID} {item.BellScheduleID} {item.Name} {item.SYS_CHANGE_VERSION} {item.SYS_CHANGE_CREATION_VERSION}");
            }
            Debug.WriteLine("---------------------------------");
        }

        private void Print(IEnumerable<AbsenceLevelReason> items)
        {
            foreach (var item in items)
            {
                Debug.WriteLine($"{item.AbsenceLevelReasonID} {item.AbsenceReasonID} {item.AbsenceLevel} {item.SYS_CHANGE_VERSION} {item.SYS_CHANGE_CREATION_VERSION}");
            }
            Debug.WriteLine("---------------------------------");
        }

        private void Print(IEnumerable<AbsenceReason> items)
        {
            foreach (var item in items)
            {
                Debug.WriteLine($"{item.AbsenceReasonID} {item.Name} {item.SYS_CHANGE_VERSION} {item.SYS_CHANGE_CREATION_VERSION}");
            }
            Debug.WriteLine("---------------------------------");
        }

        private void Print(IEnumerable<Infraction> items)
        {
            foreach (var item in items)
            {
                Debug.WriteLine($"{item.InfractionID} {item.Name} {item.VisibleInClassroom} {item.SYS_CHANGE_VERSION} {item.SYS_CHANGE_CREATION_VERSION}");
            }
            Debug.WriteLine("---------------------------------");
        }
    }

}