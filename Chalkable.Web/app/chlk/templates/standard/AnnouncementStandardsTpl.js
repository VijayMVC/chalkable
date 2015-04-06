REQUIRE('chlk.templates.standard.StandardsListTpl');

NAMESPACE('chlk.templates.standard', function(){

    /**@class chlk.templates.standard.AnnouncementStandardsTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/announcement/AnnouncementStandards.jade')],
        [ria.templates.ModelBind(chlk.models.standard.StandardTreeItem)],
        'AnnouncementStandardsTpl', EXTENDS(chlk.templates.standard.StandardsListTpl), [
            Boolean, 'ableToRemoveStandard'
        ]);
});