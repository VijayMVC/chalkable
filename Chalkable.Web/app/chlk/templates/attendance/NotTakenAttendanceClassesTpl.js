REQUIRE('chlk.models.attendance.NotTakenAttendanceClassesViewData');
REQUIRE('chlk.templates.ChlkTemplate');

NAMESPACE('chlk.templates.attendance', function () {

    /** @class chlk.templates.attendance.NotTakenAttendanceClassesTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/attendance/ClassListPage.jade')],
        [ria.templates.ModelBind(chlk.models.attendance.NotTakenAttendanceClassesViewData)],
        'NotTakenAttendanceClassesTpl', EXTENDS(chlk.templates.ChlkTemplate), [
            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.common.NameId), 'items'
        ]);
});