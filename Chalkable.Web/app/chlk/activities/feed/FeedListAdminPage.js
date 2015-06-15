REQUIRE('chlk.activities.feed.FeedListPage');
REQUIRE('chlk.templates.feed.FeedAdmin');
REQUIRE('chlk.templates.feed.NotificationsCount');
REQUIRE('chlk.templates.announcement.FeedItemTpl');

NAMESPACE('chlk.activities.feed', function () {

    /** @class chlk.activities.feed.FeedListAdminPage*/
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.feed.FeedAdmin)],
        [ria.mvc.PartialUpdateRule(chlk.templates.feed.FeedAdmin, null, null, ria.mvc.PartialUpdateRuleActions.Replace)],
        [ria.mvc.PartialUpdateRule(chlk.templates.feed.NotificationsCount, 'notifications', '.feed-notifications', ria.mvc.PartialUpdateRuleActions.Replace)],
        'FeedListAdminPage', EXTENDS(chlk.activities.feed.FeedListPage), []);
});