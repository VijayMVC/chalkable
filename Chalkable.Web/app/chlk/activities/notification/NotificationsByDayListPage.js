REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.notification.NotificationsByDateTpl');

NAMESPACE('chlk.activities.notification', function(){
   "use strict";

    /**@class chlk.activities.notification.NotificationsByDayListPage*/
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [chlk.activities.lib.PageClass('notifications')],
        [ria.mvc.TemplateBind(chlk.templates.notification.NotificationsByDateTpl)],
        'NotificationsByDayListPage', EXTENDS(chlk.activities.lib.TemplatePage),[
            [ria.mvc.DomEventBind('click', '.notification-item')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function notificationClick(node, event){
                var target = new ria.dom.Dom(event.target);
                if(!target.is('a')){
                    var link = node.find('.notification-item-action');
                    if(link.exists()){
                        link.trigger('click');
                    }
                }
            }
    ]);
});