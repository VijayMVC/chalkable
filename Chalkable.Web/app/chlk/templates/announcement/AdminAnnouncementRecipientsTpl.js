REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.announcement.Announcement');

NAMESPACE('chlk.templates.announcement', function(){

    /**@class chlk.templates.announcement.AdminAnnouncementRecipientsTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/announcement/AdminAnnouncementRecipients.jade')],
        [ria.templates.ModelBind(chlk.models.announcement.Announcement)],
        'AdminAnnouncementRecipientsTpl', EXTENDS(chlk.templates.ChlkTemplate), [

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.announcement.AdminAnnouncementRecipient), 'recipients'
        ]);
});