REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.period.ClassPeriod');

NAMESPACE('chlk.templates.calendar.announcement', function(){
    "use strict";

    /**@class chlk.templates.calendar.announcement.AdminDayCalendarPopupTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/calendar/announcement/AdminDayCalendarPopUp.jade')],
        [ria.templates.ModelBind(chlk.models.period.ClassPeriod)],
        'AdminDayCalendarPopupTpl', EXTENDS(chlk.templates.ChlkTemplate),[

            [ria.templates.ModelPropertyBind],
            chlk.models.period.Period, 'period',

            [ria.templates.ModelPropertyBind],
            chlk.models.classes.Class, 'clazz',

            [ria.templates.ModelPropertyBind],
            Number, 'roomNumber',

            [ria.templates.ModelPropertyBind],
            Number, 'studentsCount'
        ]);
});