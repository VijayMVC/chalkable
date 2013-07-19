REQUIRE('chlk.templates.JadeTemplate');
REQUIRE('chlk.models.announcement.AnnouncementForm');
REQUIRE('chlk.models.class.ClassesForTopBar');

NAMESPACE('chlk.templates.announcement', function () {

    /** @class chlk.templates.announcement.Announcement*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/announcement/AnnouncementForm.jade')],
        [ria.templates.ModelBind(chlk.models.announcement.AnnouncementForm)],
        'AnnouncementForm', EXTENDS(chlk.templates.JadeTemplate), [
            [ria.templates.ModelPropertyBind],
            chlk.models.announcement.AnnouncementId, 'id',
            [ria.templates.ModelPropertyBind],
            Number, 'announcementTypeId',
            [ria.templates.ModelPropertyBind],
            chlk.models.class.ClassesForTopBar, 'topData',
            [ria.templates.ModelPropertyBind],
            chlk.models.class.ClassForWeekMask, 'classInfo',
            [ria.templates.ModelPropertyBind],
            Number, 'selectedTypeId'
        ])
});