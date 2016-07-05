REQUIRE('chlk.controllers.BaseController');
REQUIRE('chlk.services.AnnouncementCommentService');


NAMESPACE('chlk.controllers', function (){

    /** @class chlk.controllers.AnnouncementCommentController*/
    CLASS(
        'AnnouncementCommentController', EXTENDS(chlk.controllers.BaseController), [


        [ria.mvc.Inject],
        chlk.services.AnnouncementCommentService, 'announcementCommentService',

        [chlk.controllers.NotChangedSidebarButton],
        [[chlk.models.announcement.AnnouncementComment]],
        function postAction(model){
            this.accountService
                .changePassword(model.getOldPassword(), model.getNewPassword(), model.getNewPasswordConfirmation())
                .attach(this.validateResponse_())
                .then(function(success){
                    return success
                        ? this.ShowAlertBox('Password was changed.')
                        : this.ShowAlertBox('Change password failed.');
                }, this)
                .then(function () {
                    return this.BackgroundNavigate('settings', 'dashboard', []);
                }, this);

            return null;
        }

    ])
});
