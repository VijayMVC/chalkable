REQUIRE('chlk.controllers.BaseController');
REQUIRE('chlk.services.DepartmentsService');
REQUIRE('chlk.activities.departments.DepartmentsListPage');
REQUIRE('chlk.activities.departments.AddDepartmentDialog');

NAMESPACE('chlk.controllers', function (){

    /** @class chlk.controllers.DepartmentsController */
    CLASS(
        'DepartmentsController', EXTENDS(chlk.controllers.BaseController), [

            [ria.mvc.Inject],
            chlk.services.DepartmentsService, 'departmentsService',

            [chlk.controllers.SidebarButton('settings')],
            [[Number]],
            function listAction(pageIndex_) {
                var result = this.departmentsService
                    .getDepartments(pageIndex_ | 0)
                    .attach(this.validateResponse_());
                return this.PushView(chlk.activities.departments.DepartmentsListPage, result);
            },
            [[Number]],
            function detailsAction(id){

            },
            [[Number]],
            function deleteAction(id){

            },
            function addDepartmentAction(form_){
                var model = new chlk.models.departments.Department;
                if (form_){
                    model.setName(form_.name);
                    model.setKeywords(form_.keywords.split(','));
                }
                return this.ShadeView(chlk.activities.departments.AddDepartmentDialog, ria.async.DeferredData(model));
            }

        ])
});
