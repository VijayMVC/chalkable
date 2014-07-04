REQUIRE('chlk.models.grading.ShortGradingClassSummaryGridItems');
REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.schoolYear.GradingPeriod');

NAMESPACE('chlk.templates.grading', function () {
    "use strict";
    /** @class chlk.templates.grading.ShortGradingClassSummaryGridItemsTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/grading/TeacherClassGradingGridSummaryItem.jade')],
        [ria.templates.ModelBind(chlk.models.grading.ShortGradingClassSummaryGridItems)],
        'ShortGradingClassSummaryGridItemsTpl', EXTENDS(chlk.templates.ChlkTemplate), [

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.grading.StudentWithAvg), 'students',

            [ria.templates.ModelPropertyBind],
            chlk.models.school.SchoolOption, 'schoolOptions',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.announcement.ShortAnnouncementViewData), 'gradingItems',

            [ria.templates.ModelPropertyBind],
            chlk.models.schoolYear.GradingPeriod, 'gradingPeriod',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.StandardId, 'standardId',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.AnnouncementTypeGradingId, 'categoryId',

            [ria.templates.ModelPropertyBind],
            Number, 'avg',

            [ria.templates.ModelPropertyBind],
            Boolean, 'autoUpdate',

            [ria.templates.ModelPropertyBind],
            Number, 'rowIndex',

            [ria.templates.ModelPropertyBind],
            Boolean , 'ableDisplayAlphaGrades',

            [ria.templates.ModelPropertyBind],
            Boolean , 'ableDisplayStudentAverage',

            [ria.templates.ModelPropertyBind],
            Boolean , 'ableDisplayTotalPoints',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.grading.StudentAverageInfo), 'studentAverages',

            [ria.templates.ModelPropertyBind],
            ArrayOf(Number), 'totalPoints',

            chlk.models.id.ClassId, 'classId'
        ]);
});
