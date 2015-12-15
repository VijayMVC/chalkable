REQUIRE('chlk.activities.feed.BaseFeedPage');
REQUIRE('chlk.templates.classes.ClassSummary');
REQUIRE('chlk.templates.announcement.AnnouncementsByDate');
REQUIRE('chlk.templates.announcement.FeedItemsTpl');
REQUIRE('chlk.templates.feed.Feed');

NAMESPACE('chlk.activities.classes', function () {

    /** @class chlk.activities.classes.SummaryPage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [chlk.activities.lib.PageClass('profile')],
        [ria.mvc.TemplateBind(chlk.templates.classes.ClassSummary)],
        [ria.mvc.PartialUpdateRule(chlk.templates.feed.Feed, null, '.feed-stats', ria.mvc.PartialUpdateRuleActions.Replace)],
        [ria.mvc.PartialUpdateRule(chlk.templates.announcement.FeedItemsTpl, null, '.chlk-grid', ria.mvc.PartialUpdateRuleActions.Append)],
        'SummaryPage', EXTENDS(chlk.activities.feed.BaseFeedPage), [ ]);
});