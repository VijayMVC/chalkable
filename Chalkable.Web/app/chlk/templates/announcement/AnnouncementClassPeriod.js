REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.announcement.AnnouncementClassPeriod');

NAMESPACE('chlk.templates.announcement', function () {

    /** @class chlk.templates.announcement.AnnouncementsClassPeriod*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/profile/StudentAssignmentsBlock.jade')],
        [ria.templates.ModelBind(chlk.models.announcement.AnnouncementClassPeriod)],
        'AnnouncementClassPeriod', EXTENDS(chlk.templates.ChlkTemplate), [
            [ria.templates.ModelPropertyBind],
            chlk.models.period.ClassPeriod, 'classPeriod',

            [ria.templates.ModelPropertyBind],
            [ria.serialize.SerializeProperty('daynumber')],
            Number, 'dayNumber',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.announcement.Announcement), 'announcements'
        ])
});