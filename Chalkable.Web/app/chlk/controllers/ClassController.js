REQUIRE('chlk.controllers.BaseController');
REQUIRE('chlk.services.ClassService');
REQUIRE('chlk.models.id.ClassId');

REQUIRE('chlk.activities.classes.SummaryPage');
REQUIRE('chlk.activities.classes.ClassInfoPage');

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
                return this.PushView(chlk.activities.classes.SummaryPage, result);
            },

            [[chlk.models.id.ClassId]],
            function infoAction(classId){
                var res = this.classService
                    .getInfo(classId).
                    attach(this.validateResponse_());
                return this.PushView(chlk.activities.classes.ClassInfoPage, res);
            }
        ])
});
