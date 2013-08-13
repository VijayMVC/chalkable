REQUIRE('chlk.templates.PaginatedList');
REQUIRE('chlk.models.import.School');
REQUIRE('chlk.models.import.SchoolImportViewData');
REQUIRE('chlk.models.id.DistrictId');

NAMESPACE('chlk.templates.school', function () {

    /** @class chlk.templates.school.ImportSchoolDialog*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/school/import-school-dialog.jade')],
        [ria.templates.ModelBind(chlk.models.import.SchoolImportViewData)],
        'ImportSchoolDialog', EXTENDS(chlk.templates.JadeTemplate), [
            [ria.templates.ModelPropertyBind],
            chlk.models.id.DistrictId, 'districtId',
            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.import.School), 'schools'
        ])
});