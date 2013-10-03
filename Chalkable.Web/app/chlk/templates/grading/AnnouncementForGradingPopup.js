REQUIRE('chlk.templates.announcement.Announcement');

NAMESPACE('chlk.templates.grading', function () {

    /** @class chlk.templates.grading.AnnouncementForGradingPopup*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/grading/GradingSummaryPopup.jade')],
        [ria.templates.ModelBind(chlk.models.announcement.Announcement)],
        'AnnouncementForGradingPopup', EXTENDS(chlk.templates.announcement.Announcement), [])
});