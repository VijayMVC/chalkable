REQUIRE('chlk.models.School');

NAMESPACE('chlk.templates.school', function () {

    /** @class chlk.templates.SchoolDetails*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/school/SchoolDetails.jade')],
        [ria.templates.ModelBind(chlk.models.school.SchoolDetails)],
        'SchoolDetails', EXTENDS(chlk.templates.School), [
            [ria.templates.ModelBind],
            Number, 'statusnumber',
            [ria.templates.ModelBind],
            String, 'status',
            [ria.templates.ModelBind],
            ArrayOf(Number), 'buttons',
            [ria.templates.ModelBind],
            ArrayOf(String), 'emails'
        ])
});
