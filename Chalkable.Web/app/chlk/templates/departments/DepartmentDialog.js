REQUIRE('chlk.templates.JadeTemplate');

REQUIRE('chlk.models.departments.Department');
REQUIRE('chlk.models.id.DepartmentId');

NAMESPACE('chlk.templates.departments', function () {

    /** @class chlk.templates.departments.DepartmentDialog*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/departments/department-dialog.jade')],
        [ria.templates.ModelBind(chlk.models.departments.Department)],
        'DepartmentDialog', EXTENDS(chlk.templates.JadeTemplate), [
            [ria.templates.ModelPropertyBind],
            chlk.models.id.DepartmentId, 'id',
            [ria.templates.ModelPropertyBind],
            String, 'name',
            [ria.templates.ModelPropertyBind],
            String, 'keywords'
        ])
});