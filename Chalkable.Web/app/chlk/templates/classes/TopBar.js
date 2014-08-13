REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.classes.ClassForTopBar');
REQUIRE('chlk.models.id.ClassId');

NAMESPACE('chlk.templates.classes', function () {

    /** @class chlk.templates.classes.TopBar*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/class/TopBar.jade')],
        [ria.templates.ModelBind(chlk.models.classes.ClassForTopBar)],
        'TopBar', EXTENDS(chlk.templates.ChlkTemplate), [
            [ria.templates.ModelPropertyBind],
            chlk.models.id.ClassId, 'id',
            [ria.templates.ModelPropertyBind],
            String, 'name',
            [ria.templates.ModelPropertyBind],
            String, 'classNumber',
            [ria.templates.ModelPropertyBind],
            String, 'fullClassName',
            [ria.templates.ModelPropertyBind],
            chlk.models.id.DepartmentId, 'departmentId',
            [ria.templates.ModelPropertyBind],
            Boolean, 'pressed',
            [ria.templates.ModelPropertyBind],
            Boolean, 'disabled',
            [ria.templates.ModelPropertyBind],
            Number, 'index',
            [ria.templates.ModelPropertyBind],
            String, 'controller',
            [ria.templates.ModelPropertyBind],
            String, 'action',
            [ria.templates.ModelPropertyBind],
            Array, 'params',
            [ria.templates.ModelPropertyBind],
            Number, 'defaultAnnouncementTypeId'
        ])
});