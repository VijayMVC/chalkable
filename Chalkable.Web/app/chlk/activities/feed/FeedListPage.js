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
                var ic = this.dom.find('#importan-count');
                var current = parseInt(ic.getHTML(), 10);
                if (node.parent().parent().getAttr("class").indexOf("starred") != -1)
                {
                    node.parent().parent().removeClass("starred");
                    ic.setHTML("" + (current-1));
                }
                else
                {
                    node.parent().parent().addClass("starred");
                    ic.setHTML("" + (current+1));
                }
                return true;
            },

            [ria.mvc.DomEventBind('click', '.announcement-link')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function announcementClick(node, event){
                var item = node.parent('.feed-item');
                var clone = item.clone();
                clone = clone.wrap('<div class="moving-wrapper"></div>').parent();
                clone.setCss('left', item.offset().left - 4);
                clone.setCss('top', item.offset().top);
                clone.appendTo(new ria.dom.Dom('body'));
                setTimeout(function(){
                    clone.setCss('top', 54);
                    this.dom.addClass('opacity0');
                    jQuery(document).scrollTop(0);
                }.bind(this), 1);
                setTimeout(function(){
                    clone.find('.announcement-link').removeClass('disabled').trigger('click');
                }, 301);
                return false;
            }

        ]);
});