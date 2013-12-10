REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.announcement.ItemStandards');

NAMESPACE('chlk.templates.announcement', function(){

    /**@class chlk.templates.announcement.AddStandardsTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/announcement/AddMathStandards.jade')],
        [ria.templates.ModelBind(chlk.models.announcement.ItemStandards)],
        'AddStandardsTpl', EXTENDS(chlk.templates.ChlkTemplate), [

    ]);
});