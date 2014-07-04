REQUIRE('chlk.activities.lib.TemplatePopup');
REQUIRE('chlk.templates.notification.ListNewPopupTpl');

NAMESPACE('chlk.activities.notification', function () {

    /** @class chlk.activities.notification.ListNewPopup*/
    CLASS(
        [ria.mvc.DomAppendTo('#chlk-pop-up-container')],
        [chlk.activities.lib.PopupClass('notifications')],
        [chlk.activities.lib.IsHorizontalAxis(false)],
        [chlk.activities.lib.isTopLeftPosition(false)],
        [ria.mvc.ActivityGroup('ListNewPopup')],
        [ria.mvc.TemplateBind(chlk.templates.notification.ListNewPopupTpl)],
        'ListNewPopup', EXTENDS(chlk.activities.lib.TemplatePopup), [
            [ria.mvc.DomEventBind('click', '.row')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function notificationClick(node, event){
                var target = new ria.dom.Dom(event.target);
                if(!target.is('.action-link')){
                    var link = node.find('.notification-link');
                    if(link.exists()){
                        link.trigger('click');
                    }
                }
            }
        ]);
});
