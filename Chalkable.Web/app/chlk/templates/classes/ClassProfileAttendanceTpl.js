REQUIRE('chlk.templates.profile.ClassProfileTpl');
REQUIRE('chlk.templates.calendar.attendance.ClassAttendanceMonthCalendarTpl');
REQUIRE('chlk.models.classes.ClassProfileAttendanceViewData');

NAMESPACE('chlk.templates.classes', function () {
    "use strict";
    /** @class chlk.templates.classes.ClassProfileAttendanceTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/class/ClassProfileAttendance.jade')],
        [ria.templates.ModelBind(chlk.models.classes.ClassProfileAttendanceViewData)],
        'ClassProfileAttendanceTpl', EXTENDS(chlk.templates.profile.ClassProfileTpl), [

//            [ria.templates.ModelPropertyBind],
//            chlk.models.classes.ClassAttendanceSummary, 'classAttendanceSummary',

            chlk.models.classes.ClassAttendanceSummary, function getClassAttendanceSummary(){
                return this.getClazz();
            },

            [ria.templates.ModelPropertyBind],
            chlk.models.calendar.attendance.ClassAttendanceMonthCalendar, 'monthCalendar',

            [[chlk.models.common.HoverBox.OF(chlk.models.attendance.StudentAttendanceHoverBoxItem), String]],
            Object ,function buildAttendanceGlanceBoxData(model, boxTitle){
                return {
                      value: model ? model.getTitle() : '',
                      title: boxTitle
                };
            },

            ArrayOf(Object), function buildGlanceBoxesData(){
                var summary = this.getModel().getClazz();
                return[
                    this.buildAttendanceGlanceBoxData(summary.getAbsentSection(), 'Absent'),
                    this.buildAttendanceGlanceBoxData(summary.getLateSection(), 'Late'),
                    this.buildAttendanceGlanceBoxData(summary.getPresentSection(), 'Present')
                ];
            }
        ]);
});