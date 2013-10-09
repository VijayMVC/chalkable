REQUIRE('chlk.controllers.BaseController');
REQUIRE('chlk.services.FeedService');
REQUIRE('chlk.services.NotificationService');
REQUIRE('chlk.activities.notification.ListNewPopup');
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
                    }.bind(this));

                this.notificationService.markAllAsShown();
                return this.ShadeView(chlk.activities.notification.ListNewPopup, result);
            }
        ])
});
