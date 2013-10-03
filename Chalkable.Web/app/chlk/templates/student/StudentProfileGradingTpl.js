REQUIRE('chlk.templates.profile.SchoolPersonProfileTpl');
REQUIRE('chlk.models.student.StudentProfileGradingViewData');

NAMESPACE('chlk.templates.student', function(){
    "use strict";

    /**@class chlk.templates.student.StudentProfileGradingTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/student/StudentProfileGradingView.jade')],
        [ria.templates.ModelBind(chlk.models.student.StudentProfileGradingViewData)],
        'StudentProfileGradingTpl', EXTENDS(chlk.templates.profile.SchoolPersonProfileTpl),[

            [ria.templates.ModelPropertyBind],
            chlk.models.schoolYear.MarkingPeriod, 'markingPeriod'
        ]);
});