REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.feed.Feed');
REQUIRE('chlk.templates.feed.NotificationsCount');
REQUIRE('chlk.templates.announcement.FeedItemTpl');

NAMESPACE('chlk.activities.feed', function () {

    var interval;

    /** @class chlk.activities.feed.FeedListPage*/
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.feed.Feed)],
        [ria.mvc.PartialUpdateRule(chlk.templates.feed.Feed, null, null, ria.mvc.PartialUpdateRuleActions.Replace)],
        [ria.mvc.PartialUpdateRule(chlk.templates.feed.NotificationsCount, 'notifications', '.feed-notifications', ria.mvc.PartialUpdateRuleActions.Replace)],
        'FeedListPage', EXTENDS(chlk.activities.lib.TemplatePage), [

            [ria.mvc.DomEventBind('click', '.announcement-link')],
            [[ria.dom.Dom, ria.dom.Event]],
            Boolean, function announcementClick(node, event){
                this.stopInterval();
                var item = node.parent('.feed-item');
                var clone = item.clone();
                clone.addClass('animated-item');
                clone = clone.wrap('<div class="moving-wrapper"></div>').parent();
                clone.setCss('left', item.offset().left - 4);
                clone.setCss('top', item.offset().top);
                clone.appendTo(new ria.dom.Dom('body'));
                setTimeout(function(){
                    clone.setCss('top', 54);
                    this.dom.remove();
                    jQuery(document).scrollTop(0);
                }.bind(this), 1);
                setTimeout(function(){
                    clone.find('.announcement-link').removeClass('disabled').trigger('click');
                }, 301);
                return false;
            },

            function stopInterval(){
                this.dom.find('#stop-notifications-interval').trigger('click');
            },

            OVERRIDE, VOID, function onStop_() {
                this.stopInterval();
                interval = setInterval(function(){
                    var item = new ria.dom.Dom('.moving-wrapper');
                    if(!item.exists())
                        clearInterval(interval);
                    if(!new ria.dom.Dom('.announcement-view-page').exists()){
                        item.remove();
                        clearInterval(interval);
                    }
                }, 1000);
                BASE();
                this.stopTour();
            },

            function stopTour(){
                var body = ria.dom.Dom('body');
                body.removeClass('first-login');
                body.off('click.tour');
                body.off('click.tour');
                ria.dom.Dom('.YouTubeDialog').find('.ui-dialog-titlebar-close').trigger('click');
            },

            OVERRIDE, VOID, function onRender_(model){
                BASE(model);
                if(model.isFirstLogin()){
                    var body = ria.dom.Dom('body');
                    var that = this;
                    body.addClass('first-login');
                    $("#first-login-video").YouTubePopup();
                    $("#first-login-video").trigger('click');
                    body.on('click.tour', '.YouTubeDialog .ui-dialog-titlebar-close', function(){
                        body.removeClass('first-login');
                    });
                    body.on('click.tour', function(node, event){
                        if(!node.isOrInside('.YouTubeDialog') && !node.is('.YouTubeDialog .ui-dialog-titlebar-close'))
                            that.stopTour();
                    })
                }
            }

        ]);
});