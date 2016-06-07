REQUIRE('chlk.templates.Popup');
REQUIRE('chlk.models.announcement.AnnouncementPeriod');

NAMESPACE('chlk.templates.announcement', function () {

    /** @class chlk.templates.announcement.AnnouncementPeriod*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/calendar/announcement/WeekDayPopUp.jade')],
        [ria.templates.ModelBind(chlk.models.announcement.AnnouncementPeriod)],
        'AnnouncementPeriod', EXTENDS(chlk.templates.Popup), [
            [ria.templates.ModelPropertyBind],
            chlk.models.period.Period, 'period',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.announcement.LessonPlanViewData), 'lessonPlans',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.announcement.ClassAnnouncementViewData), 'announcements',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.announcement.SupplementalAnnouncementViewData), 'supplementalAnnouncements',

            [ria.templates.ModelPropertyBind],
            chlk.models.common.ChlkDate, 'date',

            [ria.templates.ModelPropertyBind],
            Number, 'roomNumber',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.ClassId, 'selectedClassId'
        ])
});