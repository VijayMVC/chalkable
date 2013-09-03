REQUIRE('chlk.controllers.BaseController');
REQUIRE('chlk.services.ClassService');
REQUIRE('chlk.models.id.ClassId');
REQUIRE('chlk.activities.class.SummaryPage');

NAMESPACE('chlk.controllers', function (){

    /** @class chlk.controllers.ClassController */
    CLASS(
        'ClassController', EXTENDS(chlk.controllers.BaseController), [

            [ria.mvc.Inject],
            chlk.services.ClassService, 'classService',

            [[chlk.models.id.ClassId]],
            function detailsAction(classId){
                var result = this.classService
                    .getSummary(classId)
                    .attach(this.validateResponse_());
                return this.PushView(chlk.activities.class.SummaryPage, result);
            }
        ])
});
