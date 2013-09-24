REQUIRE('chlk.templates.JadeTemplate');
REQUIRE('chlk.models.classes.ClassSummary');

NAMESPACE('chlk.templates.classes', function () {

    /** @class chlk.templates.classes.ClassSummary*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/class/ClassSummary.jade')],
        [ria.templates.ModelBind(chlk.models.classes.ClassSummary)],
        'ClassSummary', EXTENDS(chlk.templates.JadeTemplate), [
            [ria.templates.ModelPropertyBind],
            chlk.models.classes.Room, 'room',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.people.User), 'students',

            [ria.templates.ModelPropertyBind],
            Number, 'classSize',

            [ria.templates.ModelPropertyBind],
            chlk.models.common.CommonHoverBox, 'classAttendanceBox',

            [ria.templates.ModelPropertyBind],
            chlk.models.common.DisciplineHoverBox, 'classDisciplineBox',

            [ria.templates.ModelPropertyBind],
            chlk.models.common.CommonHoverBox, 'classAverageBox',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.announcement.AnnouncementsByDate), 'announcementsByDate',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.ClassId, 'id',

            [ria.templates.ModelPropertyBind],
            String, 'name',

            [ria.templates.ModelPropertyBind],
            String, 'description',

            [ria.templates.ModelPropertyBind],
            chlk.models.course.Course, 'course',

            [ria.templates.ModelPropertyBind],
            chlk.models.grading.GradeLevel, 'gradeLevel',

            [ria.templates.ModelPropertyBind],
            chlk.models.people.User, 'teacher',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.id.MarkingPeriodId), 'markingPeriodsId',

            [[chlk.models.common.CommonHoverBox, String]],
            Object, function buildCommonGlanceBoxData(model, boxTitle){
                var items=[];
                var boxItems = model.getHover();
                for(var i = 0; i < boxItems.length; i++){
                    items.push({
                        data: boxItems[i],
                        getTotalMethod: boxItems[i].getTotal,
                        getSummaryMethod: boxItems[i].getSummary
                    });
                }
                return {
                    value: model.getTitle(),
                    items: items,
                    title: boxTitle
                };
            },

            Object, function buildDisciplineGlanceBoxData(){
                var items=[];
                var box = this.getModel().getClassDisciplineBox();
                var boxItems = box.getHover();
                for(var i = 0; i < boxItems.length; i++){
                    items.push({
                        data: boxItems[i],
                        getTotalMethod: boxItems[i].getCount,
                        getSummaryMethod: boxItems[i].getDisciplineName
                    });
                }
                return {
                    value: box.getTitle(),
                    items: items,
                    title: Msg.Discipline_count
                };
            }
        ])
});