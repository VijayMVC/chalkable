REQUIRE('chlk.templates.PaginatedList');
REQUIRE('chlk.models.notification.NotificationsByDate');

NAMESPACE('chlk.templates.notification', function(){
   "use strict";

    /**@class chlk.templates.notification.NotificationsByDateTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/notification/NotificationsByDateView.jade')],
        [ria.templates.ModelBind(chlk.models.common.PaginatedList)],
        'NotificationsByDateTpl', EXTENDS(chlk.templates.PaginatedList),[

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.notification.NotificationsByDate), 'items',

            [[chlk.models.common.ChlkDate]],
            String, function convertToTime(created){
                var now = new chlk.models.common.ChlkDate(getDate()), mins;
                if(now.isSameDay(created)){
                    mins = Math.floor((now.getDate() - created.getDate()) / (1000 * 60));
                    if(mins < 60) return Msg.minutes_ago(mins);
                    else return Msg.hours_ago(Math.floor(mins/60));
                }
                return created.toString('hh:mm tt');
            },

            [[chlk.models.notification.NotificationsByDate]],
            String, function getItemTitle(item){
                var now = new chlk.models.common.ChlkDate(getDate());
                var created = item.getCreated();
                return now.isSameDay(created) ? Msg.Sent_today : Msg.Sent_in_date(created.toString('MM, dd'));
            },
            [[chlk.models.notification.Notification]],
            String, function convertNotificationTypeToClassName(notification){
                var notificationEnum = chlk.models.notification.NotificationTypeEnum;
                switch(notification.getType()){
                    case notificationEnum.SIMPLE: return 'simple-notification';
                    case notificationEnum.ANNOUNCEMENT: return 'announcement-notification';
                    case notificationEnum.MESSAGE: return 'message-notification';
                    case notificationEnum.QUESTION: return 'question-notification';
                    case notificationEnum.ITEM_TO_GRADE: return 'item-to-grade-notification';
                    case notificationEnum.APP_BUDGET_BALANCE: return 'app-budget-notification';
                    case notificationEnum.APPLICATION: return 'application-notification';
                    case notificationEnum.MARKING_PERIOD_ENDING: return 'marking-period-ending-notification';
                    case notificationEnum.ATTENDANCE: return 'attendance-notification';
                    case notificationEnum.NO_TAKE_ATTENDANCE: return 'no-take-attendance-notification';
                    default: return '';
                }
            }
    ]);
});