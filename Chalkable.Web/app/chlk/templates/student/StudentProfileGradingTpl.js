REQUIRE('chlk.templates.profile.SchoolPersonProfileTpl');
REQUIRE('chlk.models.student.StudentProfileGradingViewData');

NAMESPACE('chlk.templates.student', function(){
    "use strict";

    var SJX = ria.serialize.SJX;

    /**@class chlk.templates.student.StudentProfileGradingTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/student/StudentProfileGradingView.jade')],
        [ria.templates.ModelBind(chlk.models.student.StudentProfileGradingViewData)],
        'StudentProfileGradingTpl', EXTENDS(chlk.templates.profile.SchoolPersonProfileTpl.OF(chlk.models.student.StudentGradingInfo)),[

            [ria.templates.ModelPropertyBind],
            chlk.models.schoolYear.GradingPeriod, 'gradingPeriod'
        ]);
});