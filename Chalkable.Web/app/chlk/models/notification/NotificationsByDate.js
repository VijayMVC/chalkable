REQUIRE('chlk.models.notification.Notification');

NAMESPACE('chlk.models.notification', function(){
   "use strict";
    /**@class chlk.models.notification.NotificationsByDate*/
    CLASS('NotificationsByDate', [

        chlk.models.common.ChlkDate, 'created',
        ArrayOf(chlk.models.notification.Notification), 'notifications'
    ]);
});