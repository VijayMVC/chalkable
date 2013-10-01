REQUIRE('chlk.templates.PaginatedList');
REQUIRE('chlk.models.schoolImport.School');
REQUIRE('chlk.models.schoolImport.SchoolImportViewData');
REQUIRE('chlk.models.id.DistrictId');

NAMESPACE('chlk.templates.school', function () {

    /** @class chlk.templates.school.ImportSchoolDialog*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/school/import-school-dialog.jade')],
        [ria.templates.ModelBind(chlk.models.schoolImport.SchoolImportViewData)],
        'ImportSchoolDialog', EXTENDS(chlk.templates.JadeTemplate), [
            [ria.templates.ModelPropertyBind],
            chlk.models.id.DistrictId, 'districtId',
            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.schoolImport.School), 'schools'
        ])
});