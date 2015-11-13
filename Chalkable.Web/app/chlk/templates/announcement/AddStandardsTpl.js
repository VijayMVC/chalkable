REQUIRE('chlk.templates.common.BaseAttachTpl');
REQUIRE('chlk.models.announcement.AddStandardViewData');

NAMESPACE('chlk.templates.announcement', function(){

    /**@class chlk.templates.announcement.AddStandardsTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/announcement/AddStandardsDialog.jade')],
        [ria.templates.ModelBind(chlk.models.announcement.AddStandardViewData)],
        'AddStandardsTpl', EXTENDS(chlk.templates.common.BaseAttachTpl), [

            [ria.templates.ModelPropertyBind],
            Array, 'standardIds',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.standard.StandardSubject), 'itemStandards'
    ]);
});