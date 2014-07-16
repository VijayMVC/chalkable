REQUIRE('chlk.services.BaseService');
REQUIRE('ria.async.Future');
REQUIRE('chlk.models.notification.Notification');

NAMESPACE('chlk.services', function () {
    "use strict";

    /** @class chlk.services.NotificationService */
    CLASS(
        'NotificationService', EXTENDS(chlk.services.BaseService), [
            [[Number, Number]],
            ria.async.Future, function getNotifications(start_, count_) {
                return this.getPaginatedList('Notification/List.json', chlk.models.notification.Notification, {
                    start: start_,
                    count: count_
                });
            },

            ria.async.Future, function markAllAsShown() {
                return this.get('Notification/MarkAllAsShown.json', Boolean, {});
            },
            [[Number, Number]],
            ria.async.Future, function getNotificationsByDays(start_, count_){
                return this.getPaginatedList('Notification/ListByDays.json',  chlk.models.notification.NotificationsByDate,{
                    start: start_,
                    count: count_
                });
            },

            ria.async.Future, function getUnShownNotificationCount(){
                return this.get('Notification/GetUnShownCount', Number,{});
            }
        ]);
});