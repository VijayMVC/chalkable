REQUIRE('chlk.templates.Popup');
REQUIRE('chlk.models.common.PaginatedList');
REQUIRE('chlk.models.notification.Notification');
REQUIRE('chlk.models.notification.NotificationList');

NAMESPACE('chlk.templates.notification', function () {

    /** @class chlk.templates.notification.ListNewPopupTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/notification/ListNewPopup.jade')],
        [ria.templates.ModelBind(chlk.models.notification.NotificationList)],
        'ListNewPopupTpl', EXTENDS(chlk.templates.Popup), [

            [ria.templates.ModelPropertyBind],
            chlk.models.common.PaginatedList, 'notifications'
        ])
});