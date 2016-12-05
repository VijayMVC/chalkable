REQUIRE('chlk.models.common.PageWithClasses');
REQUIRE('chlk.models.grading.ClassroomOptionViewData');
REQUIRE('chlk.models.grading.GradingScale');
REQUIRE('chlk.models.schoolYear.YearAndClasses');

NAMESPACE('chlk.models.setup', function () {
    "use strict";

    /** @class chlk.models.setup.ClassroomOptionSetupViewData*/
    CLASS('ClassroomOptionSetupViewData', EXTENDS(chlk.models.common.PageWithClasses), [

        ArrayOf(chlk.models.grading.GradingScale), 'scales',

        chlk.models.grading.ClassroomOptionViewData, 'classroomOptions',

        Boolean, 'ableCopy',

        ArrayOf(chlk.models.schoolYear.YearAndClasses), 'classesByYears',

        [[chlk.models.classes.ClassesForTopBar, ArrayOf(chlk.models.grading.GradingScale), chlk.models.grading.ClassroomOptionViewData, Boolean, ArrayOf(chlk.models.schoolYear.YearAndClasses)]],
        function $(classes_, scales_, options_, ableCopy_, classesByYears_){
            BASE(classes_);
            if(scales_)
                this.setScales(scales_);
            if(options_)
                this.setClassroomOptions(options_);
            if(ableCopy_)
                this.setAbleCopy(ableCopy_);
            if(classesByYears_)
                this.setClassesByYears(classesByYears_);
        }
    ]);
});
