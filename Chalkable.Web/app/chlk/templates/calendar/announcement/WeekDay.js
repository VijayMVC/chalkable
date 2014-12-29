REQUIRE('chlk.templates.Popup');
REQUIRE('chlk.models.calendar.announcement.WeekItem');
NAMESPACE('chlk.templates.calendar.announcement', function () {

    /** @class chlk.templates.calendar.announcement.WeekDay*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/calendar/announcement/WeekBarPopUp.jade')],
        [ria.templates.ModelBind(chlk.models.calendar.announcement.WeekItem)],
        'WeekDay', EXTENDS(chlk.templates.Popup), [
            [ria.templates.ModelPropertyBind],
            Number, 'day',

            [ria.templates.ModelPropertyBind],
            Boolean, 'sunday',

            [ria.templates.ModelPropertyBind],
            String, 'todayClassName',

            [ria.templates.ModelPropertyBind],
            chlk.models.common.ChlkDate, 'date',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.announcement.AnnouncementPeriod), 'announcementPeriods',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.announcement.Announcement), 'announcements',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.ClassId, 'selectedClassId'
        ])
});