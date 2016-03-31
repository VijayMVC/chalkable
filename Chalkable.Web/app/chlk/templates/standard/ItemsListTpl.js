REQUIRE('chlk.templates.standard.AddStandardsTpl');

NAMESPACE('chlk.templates.standard', function(){

    /**@class chlk.templates.standard.ItemsListTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/standard/ItemsList.jade')],
        [ria.templates.ModelBind(chlk.models.standard.StandardItemsListViewData)],
        'ItemsListTpl', EXTENDS(chlk.templates.standard.AddStandardsTpl), []);
});