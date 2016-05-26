REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.controls.PanoramaFilterBlockViewData');

NAMESPACE('chlk.templates.controls', function () {

    /** @class chlk.templates.controls.PanoramaFilterBlockTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/controls/panorama-filter/filter-block.jade')],
        [ria.templates.ModelBind(chlk.models.controls.PanoramaFilterBlockViewData)],
        'PanoramaFilterBlockTpl', EXTENDS(chlk.templates.ChlkTemplate), [
            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.profile.StandardizedTestViewData), 'standardizedTests',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.StandardizedTestId, 'standardizedTestId',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.StandardizedTestItemId, 'scoreTypeId',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.StandardizedTestItemId, 'componentId',

            function getCurrentTest(){
                var currentId = this.getStandardizedTestId();

                return this.getStandardizedTests().filter(function(item){
                    return item.getId() == currentId
                })[0];
            }
        ]);

    /** @class chlk.templates.controls.PanoramaFilterSelectsTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/controls/panorama-filter/filter-selects.jade')],
        [ria.templates.ModelBind(chlk.models.controls.PanoramaFilterBlockViewData)],
        'PanoramaFilterSelectsTpl', EXTENDS(chlk.templates.controls.PanoramaFilterBlockTpl), [


        ])
});