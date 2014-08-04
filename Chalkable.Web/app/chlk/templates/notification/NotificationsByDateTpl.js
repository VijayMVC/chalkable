REQUIRE('chlk.templates.PaginatedList');
REQUIRE('chlk.models.notification.NotificationsByDate');
REQUIRE('chlk.converters.notification.NotificationTypeToStyleNameConverter');

NAMESPACE('chlk.templates.notification', function(){
   "use strict";

    /**@class chlk.templates.notification.NotificationsByDateTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/notification/NotificationsByDateView.jade')],
        [ria.templates.ModelBind(chlk.models.common.PaginatedList)],
        'NotificationsByDateTpl', EXTENDS(chlk.templates.PaginatedList),[

            function $(){
                BASE();
                this._converter = new chlk.converters.notification.NotificationTypeToStyleNameConverter();
            },

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.notification.NotificationsByDate), 'items',

            [[chlk.models.notification.NotificationsByDate]],
            String, function getItemTitle(item){
                var now = new chlk.models.common.ChlkDate();
                var created = item.getCreated();
                return now.isSameDay(created) ? Msg.Sent_today : Msg.Sent_in_date(created.toString('MM, dd'));
            },
            [[chlk.models.notification.Notification]],
            String, function convertNotificationTypeToClassName(notification){
                return this._converter.convert(notification.getType());
            }
    ]);
});