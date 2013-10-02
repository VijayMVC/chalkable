REQUIRE('chlk.templates.profile.ClassProfileTpl');
REQUIRE('chlk.models.classes.ClassScheduleViewData');

NAMESPACE('chlk.templates.classes', function () {
    "use strict";
    /** @class chlk.templates.classes.ClassScheduleTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/class/ClassSchedule.jade')],
        [ria.templates.ModelBind(chlk.models.classes.ClassScheduleViewData)],
        'ClassScheduleTpl', EXTENDS(chlk.templates.profile.ClassProfileTpl), [

            [ria.templates.ModelPropertyBind],
            chlk.models.classes.Class, 'clazz',

            [ria.templates.ModelPropertyBind],
            chlk.models.calendar.announcement.Day, 'scheduleCalendar'
        ]);
});