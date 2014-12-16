REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.standard.AddCCStandardViewData');

NAMESPACE('chlk.templates.apps', function(){

    /**@class chlk.templates.apps.AddCCStandardsTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/apps/AddCCStandards.jade')],
        [ria.templates.ModelBind(chlk.models.standard.AddCCStandardViewData)],
        'AddCCStandardsTpl', EXTENDS(chlk.templates.ChlkTemplate), [

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.standard.CCStandardCategory), 'items',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.AppId, 'applicationId',

            [ria.templates.ModelPropertyBind],
            Array, 'standardCodes'
        ]);
});