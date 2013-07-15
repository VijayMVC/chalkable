REQUIRE('chlk.templates.PaginatedList');
REQUIRE('chlk.models.departments.Department');

NAMESPACE('chlk.templates.departments', function () {

    /** @class chlk.templates.departments.Departments*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/departments/Departments.jade')],
        [ria.templates.ModelBind(chlk.models.common.PaginatedList)],
        'Departments', EXTENDS(chlk.templates.PaginatedList), [
            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.departments.Department), 'items'
        ])
});