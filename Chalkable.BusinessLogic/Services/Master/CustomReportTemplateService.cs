using System;
using System.Collections.Generic;
using Chalkable.BusinessLogic.Security;
using Chalkable.Data.Common;
using Chalkable.Data.Master.Model;

namespace Chalkable.BusinessLogic.Services.Master
{
    public interface ICustomReportTemplateService
    {
        CustomReportTemplate Add(string name, string layout, string style);
        CustomReportTemplate Edit(Guid templateId, string name, string layout, string style);
        void Delete(Guid templateId);
        IList<CustomReportTemplate> GetList();
    }
    public class CustomReportTemplateService : MasterServiceBase, ICustomReportTemplateService
    {
        public CustomReportTemplateService(IServiceLocatorMaster serviceLocator) : base(serviceLocator)
        {
        }

        //implement add template icon
        public CustomReportTemplate Add(string name, string layout, string style)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            var template = new CustomReportTemplate
            {
                Id = Guid.NewGuid(),
                Name = name,
                Layout = layout,
                Style = style
            };
            DoUpdate(u => new DataAccessBase<CustomReportTemplate, Guid>(u).Insert(template));
            return template;
        }

        public CustomReportTemplate Edit(Guid templateId, string name, string layout, string style)
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
            });
            return template;
        }

        public void Delete(Guid templateId)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            DoUpdate(u=> new DataAccessBase<CustomReportTemplate, Guid>(u).Delete(templateId));
        }

        public IList<CustomReportTemplate> GetList()
        {
            return DoRead(u => new DataAccessBase<CustomReportTemplate>(u).GetAll());
        }
    }
}
