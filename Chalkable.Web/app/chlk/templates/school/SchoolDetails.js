REQUIRE('chlk.templates.school.School');
REQUIRE('chlk.models.school.SchoolDetails');

NAMESPACE('chlk.templates.school', function () {

    /** @class chlk.templates.schoool.SchoolDetails*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/school/SchoolDetails.jade')],
        [ria.templates.ModelBind(chlk.models.school.SchoolDetails)],
        'SchoolDetails', EXTENDS(chlk.templates.school.School), [
            [ria.templates.ModelBind],
            Number, 'statusNumber',
            [ria.templates.ModelBind],
            String, 'status',
            [ria.templates.ModelBind],
            ArrayOf(Number), 'buttons',
            [ria.templates.ModelBind],
            ArrayOf(String), 'emails'
        ])
});
