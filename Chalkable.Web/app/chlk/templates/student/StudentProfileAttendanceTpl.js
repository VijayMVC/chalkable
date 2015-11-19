REQUIRE('chlk.templates.profile.SchoolPersonProfileTpl');
REQUIRE('chlk.templates.calendar.attendance.StudentAttendanceMonthCalendarTpl');
REQUIRE('chlk.models.student.StudentProfileAttendanceViewData');

NAMESPACE('chlk.templates.student', function(){
   "use strict";

    /**@class chlk.templates.student.StudentProfileAttendanceTpl*/
    ASSET('~/assets/jade/activities/calendar/BaseCalendar.jade')();
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/student/StudentProfileAttendanceView.jade')],
        [ria.templates.ModelBind(chlk.models.student.StudentProfileAttendanceViewData)],
        'StudentProfileAttendanceTpl', EXTENDS(chlk.templates.profile.SchoolPersonProfileTpl.OF(chlk.models.attendance.StudentAttendanceSummary)),[

            chlk.models.attendance.StudentAttendanceSummary, function getSummaryInfo(){
                return this.getUser();
            },

            [ria.templates.ModelPropertyBind],
            chlk.models.calendar.attendance.StudentAttendanceMonthCalendar, 'attendanceCalendar',


            [[chlk.models.common.HoverBox.OF(chlk.models.attendance.StudentAttendanceHoverBoxItem), String, Boolean]],
            Object ,function buildAttendanceGlanceBoxData(model, boxTitle, noHover_){
                var items = [];
                var hoverItems = model ? model.getHover() : [];
                for(var i = 0; i < hoverItems.length; i++){
                    items.push({
                        data: hoverItems[i],
                        total: hoverItems[i].getValue() || 0,
                        summary: hoverItems[i].getClassName()
                    });
                }
                return {
                      value: model ? model.getTitle() : '',
                      items: noHover_ ? null : items,
                      title: boxTitle
                };
            },

            ArrayOf(Object), function buildGlanceBoxesData(){
                var summary = this.getModel().getSummaryInfo();
                return[
                    this.buildAttendanceGlanceBoxData(summary.getAbsentSection(), 'Absent'),
                    this.buildAttendanceGlanceBoxData(summary.getLateSection(), 'Late'),
                    this.buildAttendanceGlanceBoxData(summary.getPresentSection(), 'Present', true)
                ];
            }
        ]);
});