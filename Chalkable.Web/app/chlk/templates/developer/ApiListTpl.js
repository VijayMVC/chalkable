REQUIRE('chlk.templates.ChlkTemplate');

NAMESPACE('chlk.templates.developer', function () {

    /** @class chlk.templates.developer.ApiListTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/developer/apiList.jade')],
        [ria.templates.ModelBind(chlk.models.api.ApiListItem)],
        'ApiListTpl', EXTENDS(chlk.templates.ChlkTemplate), [

            [ria.templates.ModelPropertyBind],
            String, 'name',

            [ria.templates.ModelPropertyBind],
            String, 'role',

            [ria.templates.ModelPropertyBind],
            ArrayOf(String), 'requiredParams',

            [ria.templates.ModelPropertyBind],
            Boolean, 'method'
        ])
});



