using System;

namespace Chalkable.Data.Master.Model
{
    public class Fund
    {
        public Guid Id { get; set; }
        public DateTime PerformedDateTime { get; set; }
        public const string AMOUNT_FIELD = "Amount";
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public const string FROM_SCHOOL_REF_FIELD = "FromSchoolRef";
        public Guid? FromSchoolRef { get; set; }
        public const string TO_SCHOOL_REF_FIELD = "ToSchoolRef";
        public Guid? ToSchoolRef { get; set; }
        public const string FROM_USER_REF_FIELD = "FromUserRef";
        public Guid? FromUserRef { get; set; }
        public const string TO_USER_REF_FIELD = "ToUserRef";
        public Guid? ToUserRef { get; set; }
        public const string APP_INSTALL_ACTION_REF_FIELD = "AppInstallActionRef";
        public Guid? AppInstallActionRef { get; set; }
        public const string IS_PRIVATE_FIELD = "IsPrivate";
        public bool IsPrivate { get; set; }
        public Guid? FundRequestRef { get; set; }
        public const string SCHOOL_REF_FIELD = "SchoolRef";
        public Guid SchoolRef { get; set; }
    }

    public class FundRequest
    {
        public Guid Id { get; set; }
        public const string SCHOOL_REF_FIELD = "SchoolRef";
        public Guid? SchoolRef { get; set; }
        public const string USER_REF_FIELD = "UserRef";
        public Guid? UserRef { get; set; }
        public const string CREATED_BY_REF_FIELD = "CreatedByRef";
        public Guid CreatedByRef { get; set; }
        public decimal Amount { get; set; }
        public DateTime Created { get; set; }
        public string PurchaseOrder { get; set; }
        public Guid? SignatureRef { get; set; }
        public FundRequestState State { get; set; }
    }

    public enum FundRequestState
    {
        Created = 0,
        Confirmed = 1,
        Payed = 2,
        Rejected = 3,
    }

    public class FundRequestRoleDistribution
    {
        public Guid Id { get; set; }
        public int RoleRef { get; set; }
        public const string FUND_REQUEST_REF_FIELD = "FundRequestRef";
        public Guid FundRequestRef { get; set; }
        public decimal Amount { get; set; }
    }
}