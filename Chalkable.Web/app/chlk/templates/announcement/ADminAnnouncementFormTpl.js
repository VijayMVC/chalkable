REQUIRE('chlk.templates.announcement.BaseAnnouncementFormTpl');
REQUIRE('chlk.templates.announcement.AdminAnnouncementRecipientsTpl');
REQUIRE('chlk.models.announcement.AnnouncementCreate');

NAMESPACE('chlk.templates.announcement', function () {
    "use strict";

    ASSET('~/assets/jade/activities/announcement/BaseAnnouncementForm.jade')();

    /** @class chlk.templates.announcement.AdminAnnouncementFormTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/announcement/AdminAnnouncementForm.jade')],
        [ria.templates.ModelBind(chlk.models.announcement.AnnouncementCreate)],
        'AdminAnnouncementFormTpl', EXTENDS(chlk.templates.announcement.BaseAnnouncementFormTpl), [

        ]);
});