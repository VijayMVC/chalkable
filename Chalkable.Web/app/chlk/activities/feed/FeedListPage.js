REQUIRE('ria.mvc.TemplateActivity');
REQUIRE('chlk.templates.feed.Feed');

NAMESPACE('chlk.activities.feed', function () {

    /** @class chlk.activities.apps.FeedListPage*/
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.feed.Feed)],
        'FeedListPage', EXTENDS(ria.mvc.TemplateActivity), [ ]);
});