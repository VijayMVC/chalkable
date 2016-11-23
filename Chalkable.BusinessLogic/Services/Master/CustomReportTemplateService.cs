using System;
using System.Collections.Generic;
using Chalkable.BusinessLogic.Security;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.Master.Model;

namespace Chalkable.BusinessLogic.Services.Master
{
    public interface ICustomReportTemplateService
    {
        CustomReportTemplate Add(string name, string layout, string style, byte[] image, Guid? headerId, Guid? footerId, TemplateType type);
        CustomReportTemplate Edit(Guid templateId, string name, string layout, string style, byte[] image, Guid? headerId, Guid? footerId, TemplateType type);
        void Delete(Guid templateId);
        IList<CustomReportTemplate> GetList(TemplateType? type);
        CustomReportTemplate GetById(Guid templateId);
        byte[] GetIcon(Guid templateId);
    }
    public class CustomReportTemplateService : MasterServiceBase, ICustomReportTemplateService
    {
        public CustomReportTemplateService(IServiceLocatorMaster serviceLocator) : base(serviceLocator)
        {
        }

        private const string REPORT_TEMPLATE_ICON_CONTAINER = "report_template_icon_container"; 
        public CustomReportTemplate Add(string name, string layout, string style, byte[] icon, Guid? headerId, Guid? footerId, TemplateType type)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            var template = new CustomReportTemplate
            {
                Id = Guid.NewGuid(),
                Name = name,
                Layout = layout,
                Style = style,
                HeaderRef = headerId,
                FooterRef = footerId,
                Type = (int) type
            };
            DoUpdate(u =>
            {
                new DataAccessBase<CustomReportTemplate, Guid>(u).Insert(template);
                if(icon != null)
                    ServiceLocator.CustomReportTemplateIconService.UploadPicture(template.Id, icon);
            });
            return template;
        }

        public CustomReportTemplate Edit(Guid templateId, string name, string layout, string style, byte[] icon, Guid? headerId, Guid? footerId, TemplateType type)
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
                template.FooterRef = footerId;
                template.HeaderRef = headerId;
                template.Type = (int) type;
                da.Update(template);
                if(icon != null)
                    ServiceLocator.CustomReportTemplateIconService.UploadPicture(templateId, icon);
            });
            return template;
        }

        public byte[] GetIcon(Guid templateId)
        {
            return ServiceLocator.CustomReportTemplateIconService.GetPicture(templateId, null, null);
        }

        public void Delete(Guid templateId)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            DoUpdate(u =>
            {
                new DataAccessBase<CustomReportTemplate, Guid>(u).Delete(templateId);
                ServiceLocator.CustomReportTemplateIconService.DeletePicture(templateId);
            });
        }

        public IList<CustomReportTemplate> GetList(TemplateType? type)
        {
            var conds = new AndQueryCondition();
            if (type.HasValue)
                conds.Add(nameof(CustomReportTemplate.Type), (int) type);
            return DoRead(u => new DataAccessBase<CustomReportTemplate>(u).GetAll(conds));
        }

        public CustomReportTemplate GetById(Guid templateId)
        {
            return DoRead(u =>
            {
                var da = new DataAccessBase<CustomReportTemplate, Guid>(u);
                var res = da.GetById(templateId);
                if (res.HeaderRef.HasValue)
                    res.Header = da.GetById(res.HeaderRef.Value);
                if (res.FooterRef.HasValue)
                    res.Footer = da.GetById(res.FooterRef.Value);
                return res;
            });         
        }
    }
}
