﻿using System;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.Data.Master.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Master
{
    public class DemoApplicationUploadService : DemoMasterServiceBase, IApplicationUploadService
    {
        public DemoApplicationUploadService(IServiceLocatorMaster serviceLocator) : base(serviceLocator)
        {
        }

        public Application Create(BaseApplicationInfo appInfo)
        {
            throw new NotImplementedException();
        }

        public Application UpdateDraft(Guid applicationId, BaseApplicationInfo appInfo)
        {
            throw new NotImplementedException();
        }

        public Application Submit(Guid applicationId, BaseApplicationInfo appInfo)
        {
            throw new NotImplementedException();
        }

        public bool ApproveReject(Guid applicationId, bool isApprove)
        {
            throw new NotImplementedException();
        }

        public bool GoLive(Guid applicationId)
        {
            throw new NotImplementedException();
        }

        public bool UnList(Guid applicationId)
        {
            throw new NotImplementedException();
        }

        public bool Exists(Guid? currentApplicationId, string name, string url)
        {
            throw new NotImplementedException();
        }

        public bool DeleteApplication(Guid id)
        {
            throw new NotImplementedException();
        }

        public void ChangeApplicationType(Guid applicationId, bool isInternal)
        {
            throw new NotImplementedException();
        }


        public void SetApplicationInternalData(Guid applicationId, int? internalScore, string internalDescription)
        {
            throw new NotImplementedException();
        }
    }
}