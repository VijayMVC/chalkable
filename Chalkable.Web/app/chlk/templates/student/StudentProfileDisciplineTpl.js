REQUIRE('chlk.templates.JadeTemplate');
REQUIRE('chlk.models.student.StudentProfileDisciplineViewData');
REQUIRE('chlk.templates.calendar.discipline.StudentDisciplineMonthCalendarTpl');

NAMESPACE('chlk.templates.student', function(){
    "use strict";

    /**@class chlk.templates.student.StudentProfileDisciplineTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/student/StudentProfileDisciplineView.jade')],
        [ria.templates.ModelBind(chlk.models.student.StudentProfileDisciplineViewData)],
        'StudentProfileDisciplineTpl', EXTENDS(chlk.templates.JadeTemplate),[

            [ria.templates.ModelPropertyBind],
            chlk.models.discipline.StudentDisciplineSummary, 'summaryInfo',

            [ria.templates.ModelPropertyBind],
            chlk.models.calendar.discipline.StudentDisciplineMonthCalendar, 'disciplineCalendar',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.schoolYear.MarkingPeriod), 'markingPeriods',


            [[chlk.models.discipline.StudentDisciplineHoverBox]],
            Object ,function buildDisciplineGlanceBoxData(model){
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
                    title: model.getName(),
                    isPassing: model.isPassing()
                };
            },

            ArrayOf(Object), function buildGlanceBoxesData(){
                var discBoxes = this.getModel().getSummaryInfo().getDisciplineBoxes();
                var res = [];
                for(var i = 0; i < discBoxes.length; i++){
                    res.push(this.buildDisciplineGlanceBoxData(discBoxes[i]));
                }
                return res;
            }
        ]);
});