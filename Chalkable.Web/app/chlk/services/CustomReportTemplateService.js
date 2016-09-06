REQUIRE('chlk.services.BaseService');
REQUIRE('ria.async.Future');
REQUIRE('chlk.models.reports.CustomReportTemplate');
REQUIRE('chlk.models.id.CustomReportTemplateId');

NAMESPACE('chlk.services', function () {
    "use strict";
    /** @class chlk.services.CustomReportTemplateService*/
    CLASS(
        'CustomReportTemplateService', EXTENDS(chlk.services.BaseService), [

            [[String, String, String, Object]],
            ria.async.Future, function create(name, layout, style, files) {
                return this.uploadFiles('CustomReportTemplate/Create.json', [files[0]], chlk.models.reports.CustomReportTemplate, {
                    name: name,
                    layout: layout,
                    style: style,
                });
            },

            [[chlk.models.id.CustomReportTemplateId, String, String, String, Object]],
            ria.async.Future, function update(templateId, name, layout, style, files) {

                return this.uploadFiles('CustomReportTemplate/Update.json', [files[0]], chlk.models.reports.CustomReportTemplate, {
                    templateId: templateId.valueOf(),
                    name: name,
                    layout: layout,
                    style: style,
                });
            },

            [[chlk.models.id.CustomReportTemplateId, String, String, String, Object]],
            ria.async.Future, function saveTemplate(id_, name, layout, style, files) {
                if (id_ && id_.valueOf()) return this.update(id_, name, layout, style, files);
                return this.create(name, layout, style, files);
            },

            [[chlk.models.id.CustomReportTemplateId]],
            ria.async.Future, function removeTemplate(id) {
                return this.post('CustomReportTemplate/Delete.json', Boolean, {
                    templateId: id.valueOf()
                });
            },

            [[Number]],
            ria.async.Future, function list() {
                return this.get('CustomReportTemplate/List.json', ArrayOf(chlk.models.reports.CustomReportTemplate), {
                });
            },

            [[chlk.models.id.CustomReportTemplateId]],
            ria.async.Future, function getTemplate(id) {
                return this.get('CustomReportTemplate/Info.json', chlk.models.reports.CustomReportTemplate, {
                    templateId: id.valueOf()
                });
            }
        ])
});