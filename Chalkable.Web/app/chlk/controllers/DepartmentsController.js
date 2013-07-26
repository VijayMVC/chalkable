REQUIRE('chlk.controllers.BaseController');
REQUIRE('chlk.services.DepartmentsService');
REQUIRE('chlk.activities.departments.DepartmentsListPage');
REQUIRE('chlk.activities.departments.DepartmentDialog');
REQUIRE('chlk.models.departments.Department');

NAMESPACE('chlk.controllers', function (){

    /** @class chlk.controllers.DepartmentsController */
    CLASS(
        'DepartmentsController', EXTENDS(chlk.controllers.BaseController), [

            [ria.mvc.Inject],
            chlk.services.DepartmentsService, 'departmentService',

            [chlk.controllers.SidebarButton('settings')],

            [[Number]],
            function listAction(pageIndex_) {
                var result = this.departmentService
                    .getDepartments(pageIndex_|0)
                    .attach(this.validateResponse_());
                return this.PushView(chlk.activities.departments.DepartmentsListPage, result);
            },

            [[Number]],
            function pageAction(pageIndex) {
                var result = this.departmentService
                    .getDepartments(pageIndex)
                    .attach(this.validateResponse_());
                return this.UpdateView(chlk.activities.departments.DepartmentsListPage, result);
            },


            [[chlk.models.departments.DepartmentId]],
            function updateAction(id) {
                var result = this.departmentService
                    .getDepartment(id)
                    .attach(this.validateResponse_());
                return this.ShadeView(chlk.activities.departments.DepartmentDialog, result);
            },


            function addAction() {
                var result = new ria.async.DeferredData(new chlk.models.departments.Department);
                return this.ShadeView(chlk.activities.departments.DepartmentDialog, result);
            },

            [[chlk.models.departments.Department]],
            function saveAction(model){
                var result = this.departmentService
                    .saveDepartment(
                        model.getId(),
                        model.getName(),
                        model.getKeywords()
                    )
                    .attach(this.validateResponse_())
                    .then(function (data) {
                        this.view.getCurrent().close();
                        return this.departmentService.getDepartments(0)
                    }.bind(this));

                return this.UpdateView(chlk.activities.departments.DepartmentsListPage, result);
            },

            [[chlk.models.departments.DepartmentId]],
            function deleteAction(id) {
                var result= this.departmentService
                    .removeDepartment(id)
                    .attach(this.validateResponse_())
                    .then(function (data) {
                        return this.departmentService.getDepartments(0)
                    },this);
                return this.UpdateView(chlk.activities.departments.DepartmentsListPage, result);
            }

        ])
});
