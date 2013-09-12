using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chalkable.StiConnector.Connectors.Model
{
    public class School
    {
        public int Id { get; set;}
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public string SchoolNumber { get; set; }
    }
    
    public class AcadSession
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int AcadYear { get; set; }
        public string Description { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public int SchoolId { get; set; }
    }

    public class AcadSessionStudent
    {
        public int AcadSessionId { get; set; }
        public int Id { get; set; }
        public int? GenderId { get; set; }
        public int GradeLevelId { get; set; }

        public int? CountryOfResidenceId { get; set; }
        public int? GenerationId { get; set; }
        public int? HomelessStatusId { get; set; }
        public int? MaritalStatusId { get; set; }
        public int? MigrantFamilyId { get; set; }
        public int? NextYearSchoolId { get; set; }
        public int? ProperTitleId { get; set; }
        public int? ReligionId { get; set; }
        public string StateIdNumber { get; set; }
        public string BirthCertNumber { get; set; }
        public string BirthCertVerifyNumber { get; set; }
        public string DateOfBirth { get; set; }
        public string DisplayName { get; set; }
        public string EmployerName { get; set; }
        public string FirstName { get; set; }
        public string Guid { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string PreferredName { get; set; }
        public string SocialSecurityNumber { get; set; }
        public string StudentNumber { get; set; }
        public bool IsAllowedInetAccess { get; set; }
        public bool IsHispanic { get; set; }
        public bool IsHomeSchooled { get; set; }
        public bool IsHomeless { get; set; }
        public bool IsImmigrant { get; set; }
    }

    public class GradeLevel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Sequence { get; set; }
    }

    public class Gender
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Descriptor { get; set; }
        public string Code { get; set; }
        public bool IsActive { get; set; }
        public bool IsSystem { get; set; }
    }

    public class PersonTelephone
    {
        public string Description { get; set; }
        public bool IsListed { get; set; }
        public bool IsPrimary { get; set; }
        public int PersonId { get; set; }
        public string TelephoneNumber { get; set; }
    }

    public class PersonAddresses
    {
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressNumber { get; set; }
        public string Description { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
        public string SubdivisionName { get; set; }
        public int Id { get; set; }
        public int PersonId { get; set; }
        public int CountryId { get; set; }
        public int CountyId { get; set; }
        public int StateId { get; set; }
        public bool IsHeadOfHousehold { get; set; }
        public bool IsListed { get; set; }
        public bool IsMailing { get; set; }
        public bool IsPhysical { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    /*
     <AddressLine1>430 Plum Dr</AddressLine1>
<AddressLine2/>
<AddressNumber>A4649</AddressNumber>
<City>Abydos</City>
<CountryId>220</CountryId>
<CountyId i:nil="true"/>
<Description/>
<Id>5451</Id>
<IsHeadOfHousehold>false</IsHeadOfHousehold>
<IsListed>true</IsListed>
<IsMailing>true</IsMailing>
<IsPhysical>true</IsPhysical>
<Latitude>0.0000000</Latitude>
<Longitude>0.0000000</Longitude>
<PersonId>4951</PersonId>
<PostalCode>00000</PostalCode>
<StateId>1</StateId>
<SubdivisionName/>
     */
}
