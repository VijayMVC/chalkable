REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.announcement.FeedAnnouncementViewData');

NAMESPACE('chlk.templates.announcement', function(){

    /**@class chlk.templates.announcement.AdminAnnouncementRecipientsTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/announcement/AdminAnnouncementRecipients.jade')],
        [ria.templates.ModelBind(chlk.models.announcement.FeedAnnouncementViewData)],
        'AdminAnnouncementRecipientsTpl', EXTENDS(chlk.templates.ChlkTemplate), [

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.announcement.AdminAnnouncementRecipient), 'recipients',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.people.ShortUserInfo), 'adminAnnouncementStudents',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.AnnouncementId, 'id'
        ]);
});