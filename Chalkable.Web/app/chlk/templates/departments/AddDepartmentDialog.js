REQUIRE('chlk.templates.JadeTemplate');

REQUIRE('chlk.models.departments.Department');

NAMESPACE('chlk.templates.departments', function () {

    /** @class chlk.templates.departments.AddDepartmentDialog*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/departments/add-department-dialog.jade')],
        [ria.templates.ModelBind(chlk.models.departments.Department)],
        'AddDepartmentDialog', EXTENDS(chlk.templates.JadeTemplate), [
            [ria.templates.ModelBind],
            Number, 'id',
            [ria.templates.ModelBind],
            String, 'name',
            [ria.templates.ModelBind],
            ArrayOf(String), 'keywords'
        ])
});