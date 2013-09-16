REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.feed.Feed');

NAMESPACE('chlk.activities.feed', function () {

    /** @class chlk.activities.apps.FeedListPage*/
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.feed.Feed)],
        'FeedListPage', EXTENDS(chlk.activities.lib.TemplatePage), [

            [ria.mvc.DomEventBind('click', 'a.star')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function starAnnouncement(node, event){
                if (node.parent().parent().getAttr("class").indexOf("starred") != -1)
                    node.parent().parent().removeClass("starred");
                else
                    node.parent().parent().addClass("starred");
                return true;
            }

        ]);
});