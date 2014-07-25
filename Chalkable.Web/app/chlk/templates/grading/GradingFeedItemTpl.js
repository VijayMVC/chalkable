REQUIRE('chlk.templates.announcement.FeedItemTpl');

NAMESPACE('chlk.templates.announcement', function () {

    /** @class chlk.templates.announcement.GradingFeedItemTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/announcement/GradingFeedItem.jade')],
        [ria.templates.ModelBind(chlk.models.announcement.FeedAnnouncementViewData)],
        'FeedItemTpl', EXTENDS(chlk.templates.announcement.FeedItemTpl), []);
});