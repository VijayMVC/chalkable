REQUIRE('chlk.services.SchoolService');
REQUIRE('chlk.controllers.BaseController');
REQUIRE('chlk.activities.school.SchoolDetailsPage');


NAMESPACE('chlk.controllers', function (){

    /** @class chlk.controllers.SchoolsController */
    CLASS(
        'SchoolsController', EXTENDS(chlk.controllers.BaseController), [

        [ria.mvc.Inject],
        chlk.services.SchoolService, 'schoolService',

        [[Number]],
        function detailsAction(id) {
            var result = this.schoolService
                .getDetails(id)
                .attach(this.validateResponse_());
            return this.PushView(chlk.activities.school.SchoolDetailsPage, result);
        }
    ])
});
