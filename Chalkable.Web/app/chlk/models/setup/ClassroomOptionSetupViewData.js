REQUIRE('chlk.models.common.PageWithClasses');
REQUIRE('chlk.models.grading.ClassroomOptionViewData');
REQUIRE('chlk.models.grading.GradingScale');

NAMESPACE('chlk.models.setup', function () {
    "use strict";

    /** @class chlk.models.setup.ClassroomOptionSetupViewData*/
    CLASS('ClassroomOptionSetupViewData', EXTENDS(chlk.models.common.PageWithClasses), [

        ArrayOf(chlk.models.grading.GradingScale), 'scales',

        chlk.models.grading.ClassroomOptionViewData, 'classroomOptions',

        [[chlk.models.classes.ClassesForTopBar, ArrayOf(chlk.models.grading.GradingScale), chlk.models.grading.ClassroomOptionViewData]],
        function $(classes_, scales_, options_){
            BASE(classes_);
            if(scales_)
                this.setScales(scales_);
            if(options_)
                this.setClassroomOptions(options_);
        }
    ]);
});
