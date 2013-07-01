REQUIRE('chlk.services.SchoolService');
REQUIRE('chlk.services.AdminService');
REQUIRE('chlk.controllers.BaseController');
REQUIRE('chlk.activities.school.SchoolDetailsPage');
REQUIRE('chlk.activities.school.SchoolPeoplePage');
REQUIRE('chlk.models.school.SchoolPeople');

NAMESPACE('chlk.controllers', function (){

    /** @class chlk.controllers.SchoolsController */
    CLASS(
        'SchoolsController', EXTENDS(chlk.controllers.BaseController), [

        [ria.mvc.Inject],
        chlk.services.SchoolService, 'schoolService',

        [ria.mvc.Inject],
        chlk.services.AdminService, 'adminService',

        [[Number]],
        function detailsAction(id) {
            var result = this.schoolService
                .getDetails(id)
                .attach(this.validateResponse_());
            return this.PushView(chlk.activities.school.SchoolDetailsPage, result);
        },

        [[Number]],
        function peopleAction(id) {
            var result = ria.async.wait([
                this.schoolService.getDetails(id),
                this.adminService.getUsers(id,0)
            ]).then(function(result){
                var model = new chlk.models.school.SchoolPeople();
                model.setUsers(result[1].getItems());
                model.setSchoolInfo(result[0]);
                return model;
            });
            return this.PushView(chlk.activities.school.SchoolPeoplePage, result);
        }
    ])
});
