REQUIRE('chlk.services.SchoolService');
REQUIRE('chlk.services.AdminService');
REQUIRE('chlk.services.AccountService');
REQUIRE('chlk.services.GradeLevelService');
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

        [ria.mvc.Inject],
        chlk.services.AccountService, 'accountService',

        [ria.mvc.Inject],
        chlk.services.GradeLevelService, 'gradeLevelService',

        [[Number]],
        function detailsAction(id) {
            var result = this.schoolService
                .getDetails(id)
                .attach(this.validateResponse_());
            return this.PushView(chlk.activities.school.SchoolDetailsPage, result);
        },

        [[Number]],
        function peopleAction(id) {
            var newGradeLevels = this.gradeLevelService.getGradeLevels().slice();

            var result = ria.async.wait([
                this.schoolService.getPeopleSummary(id),
                this.adminService.getUsers(id,0),
                this.accountService.getRoles()
            ]).then(function(result){
                var serializer = new ria.serialize.JsonSerializer();
                var model = new chlk.models.school.SchoolPeople();
                model.setSchoolInfo(result[0]);
                model.setUsers(result[1].getItems());
                var roles = result[2];
                newGradeLevels.unshift(serializer.deserialize({name: 'All Grades', id: null}, chlk.models.NameId));
                roles.unshift(serializer.deserialize({name: 'All Roles', id: null}, chlk.models.NameId));
                model.setGradeLevels(newGradeLevels);
                model.setRoles(roles);
                return model;
            });
            return this.PushView(chlk.activities.school.SchoolPeoplePage, result);
        }
    ])
});
