REQUIRE('chlk.controllers.BaseController');
REQUIRE('chlk.services.AnnouncementCommentService');
REQUIRE('chlk.activities.announcement.AnnouncementViewPage');


NAMESPACE('chlk.controllers', function (){

    /** @class chlk.controllers.AnnouncementCommentController*/
    CLASS(
        'AnnouncementCommentController', EXTENDS(chlk.controllers.BaseController), [


        [ria.mvc.Inject],
        chlk.services.AnnouncementCommentService, 'announcementCommentService',

        [chlk.controllers.NotChangedSidebarButton],
        [[chlk.models.announcement.AnnouncementComment]],
        function postAction(model){
            var res = this.announcementCommentService
                .postComment(model.getAnnouncementId(), model.getText(), model.getAttachmentId())
                .attach(this.validateResponse_());

            return this.UpdateView(chlk.activities.announcement.AnnouncementViewPage, res, 'discussion');
        }

    ])
});
