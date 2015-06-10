REQUIRE('ria.templates.IConverter');
REQUIRE('chlk.models.notification.Notification');

NAMESPACE('chlk.converters.notification', function () {

    /** @class chlk.converters.notification.NotificationTypeToStyleNameConverter */
    CLASS(
        'NotificationTypeToStyleNameConverter', IMPLEMENTS(ria.templates.IConverter), [
            [[Object]],
            String, function convert(type) {
                var notificationEnum = chlk.models.notification.NotificationTypeEnum;
                VALIDATE_ARG('type', notificationEnum, type);
                switch(type){
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
        ])
});