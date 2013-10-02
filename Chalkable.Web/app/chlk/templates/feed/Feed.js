REQUIRE('chlk.models.feed.Feed');
REQUIRE('chlk.models.classes.ClassesForTopBar');
REQUIRE('chlk.models.announcement.Announcement');
REQUIRE('chlk.templates.JadeTemplate');


NAMESPACE('chlk.templates.feed', function () {

    /** @class chlk.templates.feed.Feed*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/feed/Feed.jade')],
        [ria.templates.ModelBind(chlk.models.feed.Feed)],
        'Feed', EXTENDS(chlk.templates.JadeTemplate), [
            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.announcement.Announcement), 'items',

            [ria.templates.ModelPropertyBind],
            chlk.models.classes.ClassesForTopBar, 'topData',

            [ria.templates.ModelPropertyBind],
            Boolean, 'starredOnly',

            [ria.templates.ModelPropertyBind],
            Number, 'importantCount',

            [ria.templates.ModelPropertyBind],
            Number, 'pageIndex',
            [ria.templates.ModelPropertyBind],
            Number, 'pageSize',
            [ria.templates.ModelPropertyBind],
            Number, 'totalCount',
            [ria.templates.ModelPropertyBind],
            Number, 'totalPages',
            [ria.templates.ModelPropertyBind],
            Boolean, 'hasNextPage',
            [ria.templates.ModelPropertyBind],
            Boolean, 'hasPreviousPage'
        ])
});
