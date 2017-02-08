REQUIRE('ria.dom.Dom');
REQUIRE('chlk.models.notification.Notification');
REQUIRE('chlk.models.common.PaginatedList');


NAMESPACE('chlk.models.notification', function () {

    "use strict";
    /** @class chlk.models.notification.NotificationList*/
    CLASS(
        'NotificationList', [
            chlk.models.common.PaginatedList, 'notifications',
            ria.dom.Dom, 'target',
            ria.dom.Dom, 'container'
        ]);
});

