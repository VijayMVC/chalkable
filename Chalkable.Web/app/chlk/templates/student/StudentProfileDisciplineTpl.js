REQUIRE('chlk.templates.profile.SchoolPersonProfileTpl');
REQUIRE('chlk.templates.calendar.discipline.StudentDisciplineMonthCalendarTpl');
REQUIRE('chlk.models.student.StudentProfileDisciplineViewData');

NAMESPACE('chlk.templates.student', function(){
    "use strict";

    /**@class chlk.templates.student.StudentProfileDisciplineTpl*/
    ASSET('~/assets/jade/activities/calendar/BaseCalendar.jade')();
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/student/StudentProfileDisciplineView.jade')],
        [ria.templates.ModelBind(chlk.models.student.StudentProfileDisciplineViewData)],
        'StudentProfileDisciplineTpl', EXTENDS(chlk.templates.profile.SchoolPersonProfileTpl.OF(chlk.models.discipline.StudentDisciplineSummary)),[

            chlk.models.discipline.StudentDisciplineSummary, function getSummaryInfo(){
                return this.getUser();
            },

            [ria.templates.ModelPropertyBind],
            chlk.models.calendar.discipline.StudentDisciplineMonthCalendar, 'disciplineCalendar',


            [[chlk.models.common.HoverBox.OF(chlk.models.discipline.StudentDisciplineHoverBoxItem)]],
            Object ,function buildDisciplineGlanceBoxData(model){
                return {
                    value: model.getTitle(),
                    title: model.getName(),
                    isPassing: model.isPassing()
                };
            },

            ArrayOf(Object), function buildGlanceBoxesData(){
                var discBoxes = this.getSummaryInfo().getDisciplineBoxes();
                var res = [];
                for(var i = 0; i < discBoxes.length; i++){
                    res.push(this.buildDisciplineGlanceBoxData(discBoxes[i]));
                }
                return res;
            }
        ]);
});