REQUIRE('chlk.templates.profile.SchoolPersonProfileTpl');
REQUIRE('chlk.models.student.StudentExplorerViewData');

NAMESPACE('chlk.templates.student', function(){
    "use strict";

    /**@class chlk.templates.student.StudentProfileExplorerTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/student/StudentEpxlorerView.jade')],
        [ria.templates.ModelBind(chlk.models.student.StudentExplorerViewData)],
        'StudentProfileExplorerTpl', EXTENDS(chlk.templates.profile.SchoolPersonProfileTpl.OF(chlk.models.student.StudentExplorer)),[

            [ria.templates.ModelPropertyBind],
            chlk.models.student.StudentExplorer, 'studentExplorer'

        ]);
});