REQUIRE('chlk.controllers.BaseController');
REQUIRE('chlk.services.DepartmentsService');
REQUIRE('chlk.activities.departments.DepartmentsListPage');

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

            }

        ])
});
