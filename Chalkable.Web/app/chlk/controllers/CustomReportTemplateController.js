REQUIRE('chlk.controllers.BaseController');
REQUIRE('chlk.services.CustomReportTemplateService');
REQUIRE('chlk.activities.reports.CustomReportTemplateListPage');
REQUIRE('chlk.activities.reports.CustomReportTemplateDialog');
REQUIRE('chlk.models.reports.CustomReportTemplate');
REQUIRE('chlk.models.reports.CustomReportTemplateList');
REQUIRE('chlk.models.id.CustomReportTemplateId');

NAMESPACE('chlk.controllers', function (){

    /** @class chlk.controllers.CustomReportTemplateController */
    CLASS(
        'CustomReportTemplateController', EXTENDS(chlk.controllers.BaseController), [

            [ria.mvc.Inject],
            chlk.services.CustomReportTemplateService, 'reportTemplateService',

            [chlk.controllers.SidebarButton('settings')],
            [[Number]],
            function listAction() {
                var result = this.getList_();
                return this.PushView(chlk.activities.reports.CustomReportTemplateListPage, result);
            },

            //[[Number]],
            //function pageAction(pageIndex) {
            //    var result = this.departmentService
            //        .getDepartments(pageIndex)
            //        .attach(this.validateResponse_());
            //    return this.UpdateView(chlk.activities.departments.DepartmentsListPage, result);
            //},

            ria.async.Future, function getList_(){
                return this.reportTemplateService
                    .list()
                    .attach(this.validateResponse_())
                    .then(function(templates){
                        return new chlk.models.reports.CustomReportTemplateList(templates);
                    }, this);
            },

            [[chlk.models.id.CustomReportTemplateId]],
            function updateAction(templateId) {
                var result = this.reportTemplateService
                    .getTemplate(templateId)
                    .attach(this.validateResponse_());
                return this.ShadeView(chlk.activities.reports.CustomReportTemplateDialog, result);
            },

            function addAction() {
                var result = new ria.async.DeferredData(new chlk.models.reports.CustomReportTemplate);
                return this.ShadeView(chlk.activities.reports.CustomReportTemplateDialog, result);
            },

            [[chlk.models.reports.CustomReportTemplate]],
            function saveAction(model){
                var result = this.reportTemplateService
                    .saveTemplate(
                    model.getId(),
                    model.getName(),
                    model.getLayout(),
                    model.getStyle(),
                    model.getIcon()
                )
                .attach(this.validateResponse_())
                .then(function (data) {
                    this.view.getCurrent().close();
                    return this.getList_();
                }, this);
                return this.UpdateView(chlk.activities.reports.CustomReportTemplateListPage, result);
            },

            [[chlk.models.id.CustomReportTemplateId]],
            function deleteAction(id) {
                var result= this.reportTemplateService
                    .removeTemplate(id)
                    .attach(this.validateResponse_())
                    .then(function (data) {
                        return this.getList_();
                    }, this);
                return this.UpdateView(chlk.activities.reports.CustomReportTemplateListPage, result);
            }
        ]);
});
