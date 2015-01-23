REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.announcement.Reminder');

NAMESPACE('chlk.templates.announcement', function () {

    /** @class chlk.templates.announcement.AnnouncementReminder*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/announcement/AnnouncementReminder.jade')],
        [ria.templates.ModelBind(chlk.models.announcement.Reminder)],
        'AnnouncementReminder', EXTENDS(chlk.templates.ChlkTemplate), [
            [ria.templates.ModelPropertyBind],
            Number, 'before',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.ReminderId, 'id',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.AnnouncementId, 'announcementId',

            [ria.templates.ModelPropertyBind],
            Boolean, 'owner',

            [ria.templates.ModelPropertyBind],
            chlk.models.common.ChlkDate, 'remindDate',

            [ria.templates.ModelPropertyBind],
            Boolean, 'duplicate'
        ])
});