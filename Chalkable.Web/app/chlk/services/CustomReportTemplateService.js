REQUIRE('chlk.services.BaseService');
REQUIRE('ria.async.Future');
REQUIRE('chlk.models.reports.CustomReportTemplate');
REQUIRE('chlk.models.id.CustomReportTemplateId');

NAMESPACE('chlk.services', function () {
    "use strict";
    /** @class chlk.services.CustomReportTemplateService*/
    CLASS(
        'CustomReportTemplateService', EXTENDS(chlk.services.BaseService), [

            [[String, String, String, Object
                , chlk.models.reports.CustomReportTemplateType
                , chlk.models.id.CustomReportTemplateId
                , chlk.models.id.CustomReportTemplateId]],
            ria.async.Future, function create(name, layout, style, files, type, headerId_, footerId_) {
                return this.uploadFiles('CustomReportTemplate/Create.json', files[0] ? [files[0]] : [], chlk.models.reports.CustomReportTemplate, {
                    name: name,
                    layout: layout,
                    style: style,
                    type: type.valueOf(),
                    headerId: headerId_ && headerId_.valueOf(),
                    footerId: footerId_ && footerId_.valueOf()
                });
            },

            [[chlk.models.id.CustomReportTemplateId, String, String, String, Object
                , chlk.models.reports.CustomReportTemplateType
                , chlk.models.id.CustomReportTemplateId
                , chlk.models.id.CustomReportTemplateId]],
            ria.async.Future, function update(templateId, name, layout, style, files, type, headerId_, footerId_) {

                return this.uploadFiles('CustomReportTemplate/Update.json', files[0] ? [files[0]] : [], chlk.models.reports.CustomReportTemplate, {
                    templateId: templateId.valueOf(),
                    name: name,
                    layout: layout,
                    style: style,
                    type: type.valueOf(),
                    headerId: headerId_ && headerId_.valueOf(),
                    footerId: footerId_ && footerId_.valueOf()
                });
            },

            [[chlk.models.id.CustomReportTemplateId, String, String, String, Object
                , chlk.models.reports.CustomReportTemplateType
                , chlk.models.id.CustomReportTemplateId
                , chlk.models.id.CustomReportTemplateId]],
            ria.async.Future, function saveTemplate(id_, name, layout, style, files, type, headerId_, footerId_) {
                if (id_ && id_.valueOf()) return this.update(id_, name, layout, style, files, type, headerId_, footerId_);
                return this.create(name, layout, style, files, type, headerId_, footerId_);
            },

            [[chlk.models.id.CustomReportTemplateId]],
            ria.async.Future, function removeTemplate(id) {
                return this.post('CustomReportTemplate/Delete.json', Boolean, {
                    templateId: id.valueOf()
                });
            },

            ria.async.Future, function getDefaultStudentIdToPrint(){
                return this.get('CustomReportTemplate/DefaultStudentIdToPrint.json', Number);
            },

            [[chlk.models.reports.CustomReportTemplateType]],
            ria.async.Future, function list(type_) {
                return this.get('CustomReportTemplate/List.json', ArrayOf(chlk.models.reports.CustomReportTemplate), {
                    type: type_ && type_.valueOf()
                });
            },

            ria.async.Future, function getHeaders(){
                return this.list(chlk.models.reports.CustomReportTemplateType.HEADER);
            },

            ria.async.Future, function getFooters(){
                return this.list(chlk.models.reports.CustomReportTemplateType.FOOTER);
            },

            [[chlk.models.id.CustomReportTemplateId]],
            ria.async.Future, function getTemplate(id) {
                return this.get('CustomReportTemplate/Info.json', chlk.models.reports.CustomReportTemplate, {
                    templateId: id.valueOf()
                });
            }
        ])
});