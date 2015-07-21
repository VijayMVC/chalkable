REQUIRE('chlk.templates.grading.GradingInputTpl');
REQUIRE('chlk.models.announcement.ShortStudentAnnouncementViewData');

NAMESPACE('chlk.templates.grading', function () {

    /** @class chlk.templates.grading.TeacherClassGradingGridSummaryCellTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/grading/TeacherClassGradingGridSummaryCell.jade')],
        [ria.templates.ModelBind(chlk.models.announcement.ShortStudentAnnouncementViewData)],
        'TeacherClassGradingGridSummaryCellTpl', EXTENDS(chlk.templates.grading.GradingInputTpl), [
            Number, 'maxScore',

            Number, 'index'
        ])
});