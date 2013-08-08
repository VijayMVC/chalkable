REQUIRE('chlk.templates.JadeTemplate');
REQUIRE('chlk.models.calendar.announcement.Month');
NAMESPACE('chlk.templates.calendar.announcement', function () {

    /** @class chlk.templates.calendar.announcement.MonthPage*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/calendar/announcement/MonthPage.jade')],
        [ria.templates.ModelBind(chlk.models.calendar.announcement.Month)],
        'MonthPage', EXTENDS(chlk.templates.JadeTemplate), [
        ])
});