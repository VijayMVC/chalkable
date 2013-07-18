REQUIRE('chlk.controllers.BaseController');

REQUIRE('chlk.services.AnnouncementService');
REQUIRE('chlk.activities.announcement.AnnouncementFormPage');
REQUIRE('chlk.models.announcement.Announcement');

NAMESPACE('chlk.controllers', function (){

    /** @class chlk.controllers.AnnouncementController */
    CLASS(
        'AnnouncementController', EXTENDS(chlk.controllers.BaseController), [

        [ria.mvc.Inject],
        chlk.services.AnnouncementService, 'announcementService',

        [chlk.controllers.SidebarButton('add-new')],
        //[[chlk.models.announcement.Announcement]],
        function addAction() {
            var result = new ria.async.DeferredData(new chlk.models.announcement.Announcement);
            return this.PushView(chlk.activities.announcement.AnnouncementFormPage, result);
        }
    ])
});
