REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.notification.NotificationsByDateTpl');

NAMESPACE('chlk.activities.notification', function(){
   "use strict";

    /**@class chlk.activities.notification.NotificationsByDayListPage*/
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.notification.NotificationsByDateTpl)],
        'NotificationsByDayListPage', EXTENDS(chlk.activities.lib.TemplatePage),[

    ]);
});