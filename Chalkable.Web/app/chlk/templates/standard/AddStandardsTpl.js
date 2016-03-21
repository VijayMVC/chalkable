REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.standard.StandardItemsListViewData');

NAMESPACE('chlk.templates.standard', function(){

    /**@class chlk.templates.standard.AddStandardsTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/standard/AddStandardsDialog.jade')],
        [ria.templates.ModelBind(chlk.models.standard.StandardItemsListViewData)],
        'AddStandardsTpl', EXTENDS(chlk.templates.ChlkTemplate), [

            [ria.templates.ModelPropertyBind],
            Array, 'itemIds',

            [ria.templates.ModelPropertyBind],
            String, 'requestId',

            [ria.templates.ModelPropertyBind],
            Array, 'items',

            [ria.templates.ModelPropertyBind],
            Boolean, 'onlyOne',

            [ria.templates.ModelPropertyBind],
            Array, 'selectedItems',

            [ria.templates.ModelPropertyBind],
            chlk.models.common.AttachOptionsViewData, 'attachOptions',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.standard.Breadcrumb), 'breadcrumbs',

            [ria.templates.ModelPropertyBind],
            chlk.models.standard.ItemType, 'currentItemsType'
    ]);
});