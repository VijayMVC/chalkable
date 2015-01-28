REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.standard.ApplicationStandardsViewData');

NAMESPACE('chlk.templates.standard', function(){

    /**@class chlk.templates.standard.ApplicationStandardsTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/apps/ApplicationStandards.jade')],
        [ria.templates.ModelBind(chlk.models.standard.ApplicationStandardsViewData)],
        'ApplicationStandardsTpl', EXTENDS(chlk.templates.ChlkTemplate), [

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.standard.CommonCoreStandard), 'standards',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.AppId, 'applicationId',

            Array, function getStandardsCodes(){
                var standards = this.getStandards();
                return standards && standards.map(function(standard){return standard.getStandardCode();});
            }
        ]);
});