REQUIRE('chlk.templates.standard.AddStandardsTpl');

NAMESPACE('chlk.templates.standard', function(){

    /**@class chlk.templates.standard.StandardsMainTableTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/standard/StandardsMainTable.jade')],
        [ria.templates.ModelBind(chlk.models.standard.StandardItemsListViewData)],
        'StandardsMainTableTpl', EXTENDS(chlk.templates.standard.AddStandardsTpl), []);
});