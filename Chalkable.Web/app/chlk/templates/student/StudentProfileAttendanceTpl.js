REQUIRE('chlk.templates.JadeTemplate');
REQUIRE('chlk.models.student.StudentProfileAttendanceViewData');

NAMESPACE('chlk.templates.student', function(){
   "use strict";

    /**@class chlk.templates.student.StudentProfileAttendanceTpl*/

    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/student/StudentProfileAttendanceView.jade')],
        [ria.templates.ModelBind(chlk.models.student.StudentProfileAttendanceViewData)],
        'StudentProfileAttendanceTpl', EXTENDS(chlk.templates.JadeTemplate),[

            [ria.templates.ModelPropertyBind],
            chlk.models.attendance.StudentAttendanceSummary, 'summaryInfo',

            [ria.templates.ModelPropertyBind],
            chlk.models.calendar.attendance.StudentAttendanceMonthCalendar, 'attendanceCalendar',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.schoolYear.MarkingPeriod), 'markingPeriods',


            [[chlk.models.attendance.StudentAttendanceHoverBox, String]],
            Object ,function buildAttendanceGlanceBoxData(model, boxTitle){
                var items = [];
                var hoverItems = model.getHover();
                for(var i = 0; i < hoverItems.length; i++){
                    items.push({
                        data: hoverItems[i],
                        getTotalMethod: hoverItems[i].getValue,
                        getSummaryMethod: hoverItems[i].getClassName
                    });
                }
                return {
                      value: model.getTitle(),
                      items: items,
                      title: boxTitle
                };
            },

            ArrayOf(Object), function buildGlanceBoxesData(){
                var summary = this.getModel().getSummaryInfo();
                return[
                    this.buildAttendanceGlanceBoxData(summary.getAbsentSection(), 'Absent'),
                    this.buildAttendanceGlanceBoxData(summary.getLateSection(), 'Late'),
                    this.buildAttendanceGlanceBoxData(summary.getExcusedSection(), 'Excused'),
                    this.buildAttendanceGlanceBoxData(summary.getPresentSection(), 'Present')
                ];
            }
        ]);
});