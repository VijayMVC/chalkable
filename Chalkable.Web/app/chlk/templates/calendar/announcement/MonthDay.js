REQUIRE('chlk.templates.Popup');
REQUIRE('chlk.models.calendar.announcement.MonthItem');
NAMESPACE('chlk.templates.calendar.announcement', function () {

    /** @class chlk.templates.calendar.announcement.MonthDay*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/calendar/announcement/MonthDayPopUp.jade')],
        [ria.templates.ModelBind(chlk.models.calendar.announcement.MonthItem)],
        'MonthDay', EXTENDS(chlk.templates.Popup), [
            [ria.templates.ModelPropertyBind],
            Number, 'day',

            [ria.templates.ModelPropertyBind],
            chlk.models.common.ChlkDate, 'date',

            [ria.templates.ModelPropertyBind],
            chlk.models.schoolYear.ScheduleSection, 'scheduleSection',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.announcement.Announcement), 'announcements',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.announcement.Announcement), 'items',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.ClassId, 'selectedClassId'
        ])
});