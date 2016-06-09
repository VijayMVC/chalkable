REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.standard.SelectedStandard');

NAMESPACE('chlk.templates.standard', function(){

    /**@class chlk.templates.standard.SelectedStandardTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/standard/SelectedStandard.jade')],
        [ria.templates.ModelBind(chlk.models.standard.SelectedStandard)],
        'SelectedStandardTpl', EXTENDS(chlk.templates.ChlkTemplate), [
            [ria.templates.ModelPropertyBind],
            String, 'name',

            [ria.templates.ModelPropertyBind],
            String, 'tooltip',

            [ria.templates.ModelPropertyBind],
            String, 'id',

            [ria.templates.ModelPropertyBind],
            String, 'description'
        ]);
});