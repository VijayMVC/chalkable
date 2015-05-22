NAMESPACE('chlk.models.reports', function () {
    "use strict";

    /** @class chlk.models.reports.SortActivityOptions*/
    ENUM('SortActivityOptions',{
        DATE: 0,
        CATEGORY: 1
    });

    /** @class chlk.models.reports.SortSectionOptions*/
    ENUM('SortSectionOptions',{
        TEACHER_SECTION: 0,
        TEACHER_COURSE: 1,
        TEACHER_PERIOD: 2,
        SECTION: 3
    });

    /** @class chlk.models.reports.PublicPrivateTextOptions*/
    ENUM('PublicPrivateTextOptions',{
        PUBLIC: 0,
        PRIVATE: 1,
        BOTH: 2
    });

    /** @class chlk.models.reports.SubmitLessonPlanReportViewData*/
    CLASS('SubmitLessonPlanReportViewData', EXTENDS(chlk.models.reports.BaseSubmitReportViewData), [

        chlk.models.reports.SortActivityOptions, 'sortActivities',
        chlk.models.reports.SortSectionOptions, 'sortSections',
        chlk.models.reports.PublicPrivateTextOptions, 'publicPrivateText',

        Number, 'maxCount',
        Boolean, 'includeActivities',
        Boolean, 'includeStandards',
        String, 'activityAttribute',
        String, 'activityCategory',

        [[chlk.models.id.ClassId, chlk.models.id.GradingPeriodId,
            chlk.models.common.ChlkDate, chlk.models.common.ChlkDate]],
        function $(classId_, gradingPeriodId_, startDate_, endDate_){
            BASE(classId_, gradingPeriodId_, startDate_, endDate_);
        }

    ]);
});
