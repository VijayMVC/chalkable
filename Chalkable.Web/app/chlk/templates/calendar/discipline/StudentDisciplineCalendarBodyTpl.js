REQUIRE('chlk.templates.calendar.announcement.BaseCalendarBodyTpl');
REQUIRE('chlk.models.calendar.discipline.StudentDisciplineMonthCalendar');
REQUIRE('chlk.models.common.ActionLinkModel');

NAMESPACE('chlk.templates.calendar.discipline', function (){
    "use strict";

    /**@class chlk.templates.calendar.discipline.StudentDisciplineCalendarBodyTpl*/

    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/calendar/discipline/StudentDisciplineMonthCalendarBody.jade')],
        [ria.templates.ModelBind(chlk.models.calendar.discipline.StudentDisciplineMonthCalendar)],
        'StudentDisciplineCalendarBodyTpl', EXTENDS(chlk.templates.calendar.announcement.BaseCalendarBodyTpl),[

            [ria.templates.ModelPropertyBind],
            chlk.models.id.SchoolPersonId, 'studentId',

            [ria.templates.ModelPropertyBind],
            chlk.models.common.ChlkDate, 'maxDate',

            [ria.templates.ModelPropertyBind],
            chlk.models.common.ChlkDate, 'minDate',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.calendar.discipline.StudentDisciplineCalendarMonthItem), 'calendarItems',

            [[chlk.models.calendar.discipline.StudentDisciplineCalendarMonthItem]],
            chlk.models.common.ActionLinkModel, function prepareActionLinkModel(item){
                var jsonParam = JSON.stringify([item.getDate().toStandardFormat()
                    , this.getMaxDate().toStandardFormat(), this.getMinDate().toStandardFormat()
                    , this.getStudentId().valueOf()]);
                var params = [this.getStudentId(), item.getDate().toStandardFormat(), 'students', 'disciplineMonth', jsonParam];
                return new chlk.models.common.ActionLinkModel('discipline', 'showStudentDayDisciplines', null, null, params);
            }
        ]);
});