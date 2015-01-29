REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.standard.CCStandardListViewData');

NAMESPACE('chlk.templates.standard', function(){

    /**@class chlk.templates.standard.CCStandardListTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/apps/CCStandardsColumns.jade')],
        [ria.templates.ModelBind(chlk.models.standard.CCStandardListViewData)],
        'CCStandardListTpl', EXTENDS(chlk.templates.ChlkTemplate), [

            [ria.templates.ModelPropertyBind],
            String, 'description',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.standard.CCStandardCategory), 'categories',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.CCStandardCategoryId, 'categoryId',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.standard.CommonCoreStandard), 'standards'


    ]);
});