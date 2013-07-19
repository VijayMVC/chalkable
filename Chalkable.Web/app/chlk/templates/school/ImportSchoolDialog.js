REQUIRE('chlk.templates.PaginatedList');
REQUIRE('chlk.models.school.School');

NAMESPACE('chlk.templates.school', function () {

    /** @class chlk.templates.school.ImportSchoolDialog*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/school/import-school-dialog.jade')],
        [ria.templates.ModelBind(chlk.models.common.PaginatedList)],
        'ImportSchoolDialog', EXTENDS(chlk.templates.PaginatedList), [
            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.school.School), 'items'
        ])
});