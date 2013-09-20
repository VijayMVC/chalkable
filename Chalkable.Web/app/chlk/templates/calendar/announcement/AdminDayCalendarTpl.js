REQUIRE('chlk.templates.calendar.BaseCalendarTpl');
REQUIRE('chlk.models.calendar.announcement.AdminDayCalendar');
REQUIRE('chlk.templates.calendar.announcement.AdminDayCalendarPopupTpl');

NAMESPACE('chlk.templates.calendar.announcement', function(){
    "use strict";

    /**@class chlk.templates.calendar.announcement.AdminDayCalendarTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/calendar/announcement/AdminDayCalendarPage.jade')],
        [ria.templates.ModelBind(chlk.models.calendar.announcement.AdminDayCalendar)],
        'AdminDayCalendarTpl', EXTENDS(chlk.templates.calendar.BaseCalendarTpl),[

            [ria.templates.ModelPropertyBind()],
            chlk.models.grading.GradeLevelsForTopBar, 'gradeLevelsInputData',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.grading.GradeLevel), 'gradeLevels',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.period.Period), 'periods',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.calendar.announcement.AdminDayCalendarItem), 'calendarDayItems',

            ArrayOf(ArrayOf(chlk.models.calendar.announcement.AdminDayCalendarItem)),  function getGroupedCalendarItems(){
                var model = this.getModel();
                var items = model.getCalendarDayItems();
                var gls = model.getGradeLevels();
                var groupCount = items.length / gls.length;
                var res = [], index = 0;
                for(var i = 0; i < groupCount; i++){
                    res.push(items.slice(index, index + gls.length));
                    index = (gls.length * (i + 1));
                }
                return res;
            },

            [[Object]],
            String, function getToolTip(model) {
                var tpl = new chlk.templates.calendar.announcement.AdminDayCalendarPopupTpl();
                tpl.assign(model);
                return tpl.render();
            }
        ]);
});