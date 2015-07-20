using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.Master.Model;

namespace Chalkable.Web.Models
{
    public class DistrictViewData
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        
        public int SisSystemType { get; set; }
        public string SisUrl { get; set; }
        public string SisUserName { get; set; }
        public string SisPassword { get; set; }
        public int FailCounter { get; set; }
        public int? SyncFrequency { get; set; }
        public int? MaxSyncFrequency { get; set; }
        public int FailDelta { get; set; }
        
        protected DistrictViewData(District district)
        {
            Id = district.Id;
            Name = district.Name;
            SisUrl = district.SisUrl;
            SisUserName = district.SisUserName;
            FailCounter = district.FailCounter;
            SyncFrequency = district.SyncFrequency;
            MaxSyncFrequency = district.MaxSyncFrequency;
            FailDelta = district.FailDelta;
        }

        public static DistrictViewData Create(District district)
        {
            return new DistrictViewData(district);
        }
    }

    public class DistrictSyncStatusViewData : DistrictViewData
    {
        public Guid? ProcessingId { get; set; }
        public DateTime? ProcessingCreated { get; set; }
        public DateTime? ProcessingScheduled { get; set; }
        public DateTime? ProcessingStarted { get; set; }
        public Guid? CompletedId { get; set; }
        public DateTime? CompletedCreated { get; set; }
        public DateTime? CompletedScheduled { get; set; }
        public DateTime? CompletedStarted { get; set; }
        public DateTime? CompletedCompleted { get; set; }
        public Guid? FailedId { get; set; }
        public DateTime? FailedCreated { get; set; }
        public DateTime? FailedScheduled { get; set; }
        public DateTime? FailedStarted { get; set; }
        public DateTime? FailedCompleted { get; set; }
        public Guid? CanceledId { get; set; }
        public DateTime? CanceledCreated { get; set; }
        public DateTime? CanceledScheduled { get; set; }
        public DateTime? CanceledStarted { get; set; }
        public DateTime? CanceledCompleted { get; set; }
        public Guid? NewId { get; set; }
        public DateTime? NewCreated { get; set; }
        public DateTime? NewScheduled { get; set; }

        protected DistrictSyncStatusViewData(DistrictSyncStatus district) : base(district)
        {
            ProcessingId = district.ProcessingId;
            ProcessingStarted = district.ProcessingStarted;
            ProcessingScheduled = district.ProcessingScheduled;
            ProcessingCreated = district.ProcessingCreated;

            CompletedId = district.CompletedId;
            CompletedStarted = district.CompletedStarted;
            CompletedScheduled = district.CompletedScheduled;
            CompletedCreated = district.CompletedCreated;
            CompletedCompleted = district.CompletedCompleted;

            FailedId = district.FailedId;
            FailedStarted = district.FailedStarted;
            FailedScheduled = district.FailedScheduled;
            FailedCreated = district.FailedCreated;
            FailedCompleted = district.FailedCompleted;

            CanceledId = district.CanceledId;
            CanceledStarted = district.CanceledStarted;
            CanceledScheduled = district.CanceledScheduled;
            CanceledCreated = district.CanceledCreated;
            CanceledCompleted = district.CanceledCompleted;

            NewId = district.NewId;
            NewCreated = district.NewCreated;
            NewScheduled = district.NewScheduled;
        }

        public static DistrictSyncStatusViewData Create(DistrictSyncStatus district)
        {
            return new DistrictSyncStatusViewData(district);
        }

        public static IList<DistrictSyncStatusViewData> Create(IList<DistrictSyncStatus> districts)
        {
            return districts.Select(Create).ToList();
        } 
    }

    public class DistrictRegisterViewData
    {
        public Guid LinkKey { get; set; }
        public Guid DistrictGuid { get; set; }
        public string ApiUrl { get; set; }
        public string RedirectUrl { get; set; }
        public string SisUserName { get; set; }
        public string SisPassword { get; set; }
        public string DistrictTimeZone { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string DistrictState { get; set; }
    }

}