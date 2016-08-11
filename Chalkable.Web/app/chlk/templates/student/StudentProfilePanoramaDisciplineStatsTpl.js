REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.panorama.StudentPanoramaViewData');

NAMESPACE('chlk.templates.student', function () {

    /** @class chlk.templates.student.StudentProfilePanoramaDisciplineStatsTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/student/StudentProfilePanoramaDisciplineStats.jade')],
        [ria.templates.ModelBind(chlk.models.panorama.StudentPanoramaViewData)],
        'StudentProfilePanoramaDisciplineStatsTpl', EXTENDS(chlk.templates.ChlkTemplate), [

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.panorama.StudentDisciplineStatViewData), 'studentDisciplineStats',

            [ria.templates.ModelPropertyBind],
            chlk.models.panorama.StudentDisciplinesSortType, 'disciplinesOrderBy',

            [ria.templates.ModelPropertyBind],
            Boolean, 'disciplinesDescending'
        ]);
});