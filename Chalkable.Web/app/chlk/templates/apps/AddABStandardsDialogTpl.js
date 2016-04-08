REQUIRE('chlk.templates.standard.AddStandardsTpl');

NAMESPACE('chlk.templates.apps', function(){

    /**@class chlk.templates.apps.AddABStandardsDialogTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/apps/AddABStandardsDialog.jade')],
        [ria.templates.ModelBind(chlk.models.standard.StandardItemsListViewData)],
        'AddABStandardsDialogTpl', EXTENDS(chlk.templates.standard.AddStandardsTpl), [

        ]);
});
