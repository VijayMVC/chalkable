REQUIRE('chlk.templates.JadeTemplate');
REQUIRE('chlk.models.announcement.AnnouncementForm');
REQUIRE('chlk.models.classes.ClassesForTopBar');

NAMESPACE('chlk.templates.announcement', function () {

    /** @class chlk.templates.announcement.AnnouncementForm*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/announcement/AnnouncementForm.jade')],
        [ria.templates.ModelBind(chlk.models.announcement.AnnouncementForm)],
        'AnnouncementForm', EXTENDS(chlk.templates.JadeTemplate), [
            [ria.templates.ModelPropertyBind],
            chlk.models.announcement.Announcement, 'announcement',

            [ria.templates.ModelPropertyBind],
            chlk.models.classes.ClassesForTopBar, 'topData',

            [ria.templates.ModelPropertyBind],
            chlk.models.classes.ClassForWeekMask, 'classInfo',

            [ria.templates.ModelPropertyBind],
            Number, 'selectedTypeId',

            [ria.templates.ModelPropertyBind],
            Boolean, 'isDraft',

            [ria.templates.ModelPropertyBind],
            Array, 'recipients',

            [ria.templates.ModelPropertyBind],
            Array, 'reminders'
        ])
});