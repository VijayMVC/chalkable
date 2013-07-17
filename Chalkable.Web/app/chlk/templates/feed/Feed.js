REQUIRE('chlk.templates.PaginatedList');
REQUIRE('chlk.models.feed.Announcement');

NAMESPACE('chlk.templates.feed', function () {

    /** @class chlk.templates.feed.Feed*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/feed/Feed.jade')],
        [ria.templates.ModelBind(chlk.models.common.PaginatedList)],
        'Feed', EXTENDS(chlk.templates.PaginatedList), [
            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.feed.Announcement), 'items'
        ])
});