﻿using System;
using Chalkable.Data.Common;

namespace Chalkable.Data.Master.Model
{
    public class District
    {
        public const string ID_FIELD = "Id";
        public const string SERVER_URL_FIELD = "ServerUrl";
        public const string SIS_URL_FIELD = "SisUrl";
        public const string SIS_DISTRICT_IF_FIELD = "SisDistrictId";
        public const string LAST_SYNC_FIELD = "LastSync";

        [PrimaryKeyFieldAttr]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string SisUrl { get; set; }
        public string SisUserName { get; set; }
        public string SisPassword { get; set; }
        public int Status { get; set; }
        public string TimeZone { get; set; }
        public string ServerUrl { get; set; }
        public string SisRedirectUrl { get; set; }
        public DateTime? LastSync { get; set; }
        public int MaxSyncTime { get; set; }
        public int SyncLogFlushSize { get; set; }
        public int SyncHistoryDays { get; set; }
        
        [NotDbFieldAttr]
        public bool IsDemoDistrict { get; set; }
    }

    public enum DistrictStatus
    {
        Created = 0,
        DataImported = 1,
        InvitedUser = 2,
        TeacherLogged = 3,
        InvitedStudent = 4,
        StudentLogged = 5,
        PayingCustomer = 6
    }

    public class DistrictSyncStatus : District
    {
        public Guid? ProcessingId { get; set; }
		public DateTime? ProcessingCreated { get; set; }
		public DateTime? ProcessingStarted { get; set; }
		public Guid? CompletedId { get; set; }
		public DateTime? CompletedCreated { get; set; }
		public DateTime? CompletedStarted { get; set; }
		public DateTime? CompletedCompleted { get; set; }
		public Guid? FailedId { get; set; }
		public DateTime? FailedCreated { get; set; }
		public DateTime? FailedStarted { get; set; }
        public DateTime? FailedCompleted { get; set; }
        public Guid? CanceledId { get; set; }
        public DateTime? CanceledCreated { get; set; }
        public DateTime? CanceledStarted { get; set; }
        public DateTime? CanceledCompleted { get; set; }
    }
}