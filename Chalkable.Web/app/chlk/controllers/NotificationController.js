REQUIRE('chlk.controllers.BaseController');
REQUIRE('chlk.services.FeedService');
REQUIRE('chlk.services.NotificationService');
REQUIRE('chlk.activities.notification.ListNewPopup');
REQUIRE('chlk.activities.notification.NotificationsByDayListPage');
REQUIRE('chlk.models.notification.NotificationList');


NAMESPACE('chlk.controllers', function (){

    /** @class chlk.controllers.NotificationController*/
    CLASS(
        'NotificationController', EXTENDS(chlk.controllers.BaseController), [

            [ria.mvc.Inject],
            chlk.services.NotificationService, 'notificationService',

            [ria.mvc.Inject],
            chlk.services.FeedService, 'feedService',

            function listNewAction() {
                var result = this.notificationService.getNotifications()
                    .attach(this.validateResponse_())
                    .then(function(model){
                        var res = new chlk.models.notification.NotificationList();
                        res.setNotifications(model);
                        res.setTarget(chlk.controls.getActionLinkControlLastNode());
                        return res;
                    }, this);

                this.notificationService.markAllAsShown();
                return this.ShadeView(chlk.activities.notification.ListNewPopup, result);
            },

            [[Number, Number, Boolean]],
            function listByDaysAction(start_, count_){
                var activityCls = chlk.activities.notification.NotificationsByDayListPage;
                var res = this.notificationService
                    .getNotificationsByDays(start_, count_)
                    .attach(this.validateResponse_());
                return this.isPushed_(activityCls)
                    ? this.UpdateView(activityCls, res)
                    : this.PushView(activityCls, res);
            },

            [[Object]],
            Boolean, function isPushed_(activityCls){
                var currentActivity = this.getView().getCurrent();
                return currentActivity && currentActivity.getClass() == activityCls;
            }
        ])
});
