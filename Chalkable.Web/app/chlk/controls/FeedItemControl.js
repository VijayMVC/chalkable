REQUIRE('chlk.controls.Base');

NAMESPACE('chlk.controls', function () {

    /** @class chlk.controls.FeedItemControl*/
    CLASS(
        'FeedItemControl', EXTENDS(chlk.controls.Base), [
            OVERRIDE, VOID, function onCreate_() {
                BASE();
                ASSET('~/assets/jade/controls/feed-item.jade')(this);
            },

            [ria.mvc.DomEventBind('click', '.feed-item a.star')],
            [[ria.dom.Dom, ria.dom.Event]],
            Boolean, function starClick(node, event){
                var feedItem = node.parent('.feed-item');
                if (feedItem.getAttr("class").indexOf("complete") != -1)
                {
                    feedItem.removeClass("complete");
                    //node.setData('tooltip', Msg.ToDo);
                }
                else
                {
                    feedItem.addClass("complete");
                    //node.setData('tooltip', Msg.Done);
                }
                return true;
            }
        ]);
});