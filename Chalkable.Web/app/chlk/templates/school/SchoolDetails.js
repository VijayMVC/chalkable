REQUIRE('chlk.templates.school.School');
REQUIRE('chlk.models.school.SchoolDetails');

NAMESPACE('chlk.templates.school', function () {

    /** @class chlk.templates.schoool.SchoolDetails*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/school/SchoolDetails.jade')],
        [ria.templates.ModelBind(chlk.models.school.SchoolDetails)],
        'SchoolDetails', EXTENDS(chlk.templates.school.School), [
            [ria.templates.ModelPropertyBind],
            Number, 'statusNumber',
            [ria.templates.ModelPropertyBind],
            String, 'status',
            [ria.templates.ModelPropertyBind],
            ArrayOf(Number), 'buttons',
            [ria.templates.ModelPropertyBind],
            ArrayOf(String), 'emails'
        ])
});
