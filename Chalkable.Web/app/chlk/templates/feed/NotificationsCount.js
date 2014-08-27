REQUIRE('chlk.templates.feed.Feed');

NAMESPACE('chlk.templates.feed', function () {

    /** @class chlk.templates.feed.NotificationsCount*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/feed/NotificationsCount.jade')],
        [ria.templates.ModelBind(chlk.models.feed.Feed)],
        'NotificationsCount', EXTENDS(chlk.templates.feed.Feed), [])
});
