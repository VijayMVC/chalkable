REQUIRE('chlk.templates.standard.AddStandardsTpl');

NAMESPACE('chlk.templates.apps', function(){

    /**@class chlk.templates.apps.AddCommonCoreStandardsDialogTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/apps/AddCommonCoreStandardsDialog.jade')],
        [ria.templates.ModelBind(chlk.models.standard.StandardItemsListViewData)],
        'AddCommonCoreStandardsDialogTpl', EXTENDS(chlk.templates.standard.AddStandardsTpl), [

        ]);
});
