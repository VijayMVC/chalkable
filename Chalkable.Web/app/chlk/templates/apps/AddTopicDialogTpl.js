REQUIRE('chlk.templates.standard.AddStandardsTpl');

NAMESPACE('chlk.templates.apps', function(){

    /**@class chlk.templates.apps.AddTopicDialogTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/standard/AddTopicDialog.jade')],
        [ria.templates.ModelBind(chlk.models.standard.StandardItemsListViewData)],
        'AddTopicDialogTpl', EXTENDS(chlk.templates.standard.AddStandardsTpl), [

        ]);
});
