REQUIRE('chlk.templates.announcement.FeedItemTpl');

NAMESPACE('chlk.templates.announcement', function () {

    /** @class chlk.templates.grading.GradingFeedItemTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/announcement/GradingFeedItem.jade')],
        [ria.templates.ModelBind(chlk.models.announcement.FeedAnnouncementViewData)],
        'GradingFeedItemTpl', EXTENDS(chlk.templates.announcement.FeedItemTpl), []);
});