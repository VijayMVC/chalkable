REQUIRE('chlk.templates.grading.GradingInputTpl');
REQUIRE('chlk.models.announcement.ShortStudentAnnouncementViewData');

NAMESPACE('chlk.templates.grading', function () {

    /** @class chlk.templates.grading.GradingPopUpTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/grading/GradingPopUp.jade')],
        [ria.templates.ModelBind(chlk.models.announcement.ShortStudentAnnouncementViewData)],
        'GradingPopUpTpl', EXTENDS(chlk.templates.grading.GradingInputTpl), [])
});