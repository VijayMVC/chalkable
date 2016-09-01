REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.panorama.StudentPanoramaViewData');

NAMESPACE('chlk.templates.student', function () {

    /** @class chlk.templates.student.StudentProfilePanoramaAttendanceStatsTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/student/StudentProfilePanoramaAttendanceStats.jade')],
        [ria.templates.ModelBind(chlk.models.panorama.StudentPanoramaViewData)],
        'StudentProfilePanoramaAttendanceStatsTpl', EXTENDS(chlk.templates.ChlkTemplate), [

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.panorama.StudentAbsenceStatViewData), 'studentAbsenceStats',

            [ria.templates.ModelPropertyBind],
            chlk.models.panorama.StudentAttendancesSortType, 'attendancesOrderBy',

            [ria.templates.ModelPropertyBind],
            Boolean, 'attendancesDescending'
        ]);
});