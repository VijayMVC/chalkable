REQUIRE('chlk.templates.JadeTemplate');
REQUIRE('chlk.models.announcement.AnnouncementsByDate');

NAMESPACE('chlk.templates.announcement', function () {

    /** @class chlk.templates.announcement.AnnouncementsByDate*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/profile/AssignmentsBlock.jade')],
        [ria.templates.ModelBind(chlk.models.announcement.AnnouncementsByDate)],
        'AnnouncementsByDate', EXTENDS(chlk.templates.JadeTemplate), [
            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.announcement.Announcement), 'announcements',

            [ria.templates.ModelPropertyBind],
            chlk.models.common.ChlkDate, 'date',

            [ria.templates.ModelPropertyBind],
            Number, 'day'
        ])
});