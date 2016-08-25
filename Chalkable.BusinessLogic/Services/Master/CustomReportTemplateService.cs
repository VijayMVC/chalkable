﻿using System;
using System.Collections.Generic;
using Chalkable.BusinessLogic.Security;
using Chalkable.Data.Common;
using Chalkable.Data.Master.Model;

namespace Chalkable.BusinessLogic.Services.Master
{
    public interface ICustomReportTemplateService
    {
        CustomReportTemplate Add(string name, string layout, string style, byte[] image);
        CustomReportTemplate Edit(Guid templateId, string name, string layout, string style, byte[] image);
        void Delete(Guid templateId);
        IList<CustomReportTemplate> GetList();
        CustomReportTemplate GetById(Guid templateId);
        byte[] GetIcon(Guid templateId);
    }
    public class CustomReportTemplateService : MasterServiceBase, ICustomReportTemplateService
    {
        public CustomReportTemplateService(IServiceLocatorMaster serviceLocator) : base(serviceLocator)
        {
        }

        private const string REPORT_TEMPLATE_ICON_CONTAINER = "report_template_icon_container"; 
        public CustomReportTemplate Add(string name, string layout, string style, byte[] icon)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            var template = new CustomReportTemplate
            {
                Id = Guid.NewGuid(),
                Name = name,
                Layout = layout,
                Style = style
            };
            DoUpdate(u =>
            {
                new DataAccessBase<CustomReportTemplate, Guid>(u).Insert(template);
                UploadIcon(template.Id, icon);
            });
            return template;
        }

        public CustomReportTemplate Edit(Guid templateId, string name, string layout, string style, byte[] icon)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            CustomReportTemplate template = null;
            DoUpdate(u =>
            {
                var da = new DataAccessBase<CustomReportTemplate, Guid>(u);
                template = da.GetById(templateId);
                template.Layout = layout;
                template.Style = style;
                template.Name = name;
                da.Update(template);
                UploadIcon(template.Id, icon);
            });
            return template;
        }

        public byte[] GetIcon(Guid templateId)
        {
            return ServiceLocator.StorageBlobService.GetBlobContent(REPORT_TEMPLATE_ICON_CONTAINER, templateId.ToString());
        }
        private void UploadIcon(Guid templateId, byte[] icon)
        {
            ServiceLocator.StorageBlobService.AddBlob(REPORT_TEMPLATE_ICON_CONTAINER, templateId.ToString(), icon);
        }

        private void DeleteIcon(Guid templateId)
        {
            ServiceLocator.StorageBlobService.DeleteBlob(REPORT_TEMPLATE_ICON_CONTAINER, templateId.ToString());
        }
        

        public void Delete(Guid templateId)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            DoUpdate(u =>
            {
                new DataAccessBase<CustomReportTemplate, Guid>(u).Delete(templateId);
                DeleteIcon(templateId);
            });
        }

        public IList<CustomReportTemplate> GetList()
        {
            return DoRead(u => new DataAccessBase<CustomReportTemplate>(u).GetAll());
        }

        public CustomReportTemplate GetById(Guid templateId)
        {
            return DoRead(u => new DataAccessBase<CustomReportTemplate, Guid>(u).GetById(templateId));
        }

    }
}
