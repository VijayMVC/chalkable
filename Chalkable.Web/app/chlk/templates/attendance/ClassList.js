REQUIRE('chlk.templates.common.PageWithClasses');
REQUIRE('chlk.models.attendance.ClassList');

REQUIRE('chlk.converters.ClassAttendanceIdToNameConverter');

NAMESPACE('chlk.templates.attendance', function () {

    /** @class chlk.templates.attendance.ClassList*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/attendance/ClassListPage.jade')],
        [ria.templates.ModelBind(chlk.models.attendance.ClassList)],
        'ClassList', EXTENDS(chlk.templates.common.PageWithClasses), [
            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.attendance.ClassAttendance), 'items',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.attendance.AttendanceReason), 'reasons',

            [ria.templates.ModelPropertyBind],
            chlk.models.common.ChlkDate, 'date',

            [ria.templates.ModelPropertyBind],
            Boolean, 'byLastName'
        ])
});