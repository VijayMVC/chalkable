REQUIRE('chlk.models.feed.FeedItems');
REQUIRE('chlk.templates.ChlkTemplate');

NAMESPACE('chlk.templates.announcement', function () {

    /** @class chlk.templates.announcement.FeedItemsTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/announcement/FeedItems.jade')],
        [ria.templates.ModelBind(chlk.models.feed.FeedItems)],
        'FeedItemsTpl', EXTENDS(chlk.templates.ChlkTemplate), [
            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.announcement.FeedAnnouncementViewData), 'items',

            [ria.templates.ModelPropertyBind],
            Boolean, 'readonly'

        ]);
});