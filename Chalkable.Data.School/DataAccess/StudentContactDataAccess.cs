using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.Model;
using Chalkable.Data.School.Model.Sis;

namespace Chalkable.Data.School.DataAccess
{
    public class StudentContactDataAccess : DataAccessBase<StudentContact, int>
    {
        public StudentContactDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
        
        //TODO: maybe move this to procedure later
        public IList<StudentContactDetails> GetStudentContactsDetails(int studentId)
        {
            var stcQuery = new DbQuery();
            var types = new List<Type> {typeof (StudentContact), typeof (Person), typeof (ContactRelationship)};
            stcQuery.Sql.AppendFormat(@" select {0} from [{1}]
                                         join [{4}] on [{4}].[{5}] = [{1}].[{2}]
                                         join [{6}] on [{6}].[{7}] = [{1}].[{3}] "
                                      , Orm.ComplexResultSetQuery(types), types[0].Name, StudentContact.CONTACT_REF_FIELD,
                                      StudentContact.CONTACT_RELATIONSHIP_REF_FIELD,
                                      types[1].Name, Person.ID_FIELD, types[2].Name, ContactRelationship.ID_FIELD);

            var conds = new AndQueryCondition { { StudentContact.STUDENT_REF_FIELD, studentId } };
            conds.BuildSqlWhere(stcQuery, Orm.TableName(types[0]));

            var addressQuery = new DbQuery();
            addressQuery.Sql.AppendFormat($@" select {Orm.TableName(typeof(Address))}.* from {Orm.TableName(typeof(Address))}
                                             join {Orm.TableName(typeof(Person))} on {Orm.TableName(typeof(Person))}.AddressRef = {Orm.TableName(typeof(Address))}.Id
                                             join {Orm.TableName(typeof(StudentContact))} on {Orm.TableName(typeof(StudentContact))}.ContactRef = {Orm.TableName(typeof(Person))}.Id ");
            conds.BuildSqlWhere(addressQuery, Orm.TableName(types[0]));

            var phonesQuery = new DbQuery();
            phonesQuery.Sql.AppendFormat($@" select {Orm.TableName(typeof(Phone))}.* from {Orm.TableName(typeof(Phone))}
                                             join {Orm.TableName(typeof(StudentContact))} on {Orm.TableName(typeof(StudentContact))}.ContactRef = {Orm.TableName(typeof(Phone))}.PersonRef ");
            conds.BuildSqlWhere(phonesQuery, Orm.TableName(types[0]));

            var emailsQuery = new DbQuery();
            emailsQuery.Sql.AppendFormat($@" select {Orm.TableName(typeof(PersonEmail))}.* from {Orm.TableName(typeof(PersonEmail))}
                                             join {Orm.TableName(typeof(StudentContact))} on {Orm.TableName(typeof(StudentContact))}.ContactRef = {Orm.TableName(typeof(PersonEmail))}.PersonRef ");
            conds.BuildSqlWhere(emailsQuery, Orm.TableName(types[0]));

            var dbQeury = new DbQuery(new List<DbQuery> {stcQuery, addressQuery, phonesQuery, emailsQuery});

            return Read(dbQeury, ReadStudentContactDetailsRes);
        }

        private IList<StudentContactDetails> ReadStudentContactDetailsRes(DbDataReader reader)
        {
            var studentContacts = reader.ReadList<StudentContactDetails>(true);
            if (studentContacts != null && studentContacts.Count > 0)
            {
                reader.NextResult();
                var addresses = reader.ReadList<Address>();
                reader.NextResult();
                var phones = reader.ReadList<Phone>();
                reader.NextResult();
                var emails = reader.ReadList<PersonEmail>();
                foreach (var studentContact in studentContacts)
                {
                    studentContact.Person.Address = addresses.FirstOrDefault(a => a.Id == studentContact.Person.AddressRef);
                    studentContact.Person.Phones = phones.Where(p => p.PersonRef == studentContact.ContactRef).ToList();
                    studentContact.Person.PersonEmails = emails.Where(a => a.PersonRef == studentContact.ContactRef).ToList();
                }
            }
            return studentContacts.OrderByDescending(x => x.IsFamilyMember).ThenBy(x => x.Person.LastName).ThenBy(x => x.Person.FirstName).ToList();
        } 
    }
}
