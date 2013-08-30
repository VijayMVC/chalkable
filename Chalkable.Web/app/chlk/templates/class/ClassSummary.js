REQUIRE('chlk.templates.JadeTemplate');
REQUIRE('chlk.models.class.ClassSummary');

NAMESPACE('chlk.templates.class', function () {

    /** @class chlk.templates.class.ClassSummary*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/class/ClassSummary.jade')],
        [ria.templates.ModelBind(chlk.models.class.ClassSummary)],
        'ClassSummary', EXTENDS(chlk.templates.JadeTemplate), [
            [ria.templates.ModelPropertyBind],
            chlk.models.class.Room, 'room',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.people.User), 'students',

            [ria.templates.ModelPropertyBind],
            Number, 'classSize',

            [ria.templates.ModelPropertyBind],
            chlk.models.common.HoverBox, 'classAttendanceBox',

            [ria.templates.ModelPropertyBind],
            chlk.models.common.HoverBox, 'classDisciplineBox',

            [ria.templates.ModelPropertyBind],
            chlk.models.common.HoverBox, 'classAverageBox',

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
            ArrayOf(chlk.models.id.MarkingPeriodId), 'markingPeriodsId'
        ])
});