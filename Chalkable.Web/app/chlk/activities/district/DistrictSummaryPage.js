REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.district.DistrictSummaryTpl');
REQUIRE('chlk.templates.district.BaseStatisticTpl');
REQUIRE('chlk.templates.district.BaseStatisticItemsTpl');

NAMESPACE('chlk.activities.district', function () {
    var filterTimeout;

    /** @class chlk.activities.district.DistrictSummaryPage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.district.DistrictSummaryTpl)],
        [ria.mvc.PartialUpdateRule(chlk.templates.district.BaseStatisticTpl, '', '.grid-container', ria.mvc.PartialUpdateRuleActions.Replace)],
        [ria.mvc.PartialUpdateRule(chlk.templates.district.BaseStatisticItemsTpl, chlk.activities.lib.DontShowLoader(), '.grid-content', ria.mvc.PartialUpdateRuleActions.Append)],
        'DistrictSummaryPage', EXTENDS(chlk.activities.lib.TemplatePage), [

            [ria.mvc.DomEventBind('keyup change', '.statistic-filter')],
            [[ria.dom.Dom, ria.dom.Event]],
            function filterKeyUp(node, event){
                clearTimeout(filterTimeout);
                var time = event.type == 'change' ? 1 : 1000;
                var btn = this.dom.find('.filter-submit');

                filterTimeout = setTimeout(function(){
                    var val = node.getValue();
                    if(val != node.getData('value')){
                        btn.trigger('click');
                        node.setData('value', val);
                    }

                }, time);
            },

            OVERRIDE, VOID, function onPartialRefresh_(model, msg_) {
                BASE(model, msg_);
                if(model instanceof chlk.models.admin.BaseStatisticGridViewData){
                    this.dom.find('.district-summary-grid').trigger(chlk.controls.GridEvents.UPDATED.valueOf());
                    if(!model.getItems().length)
                        this.dom.find('.school-statistic-form').trigger(chlk.controls.FormEvents.DISABLE_SCROLLING.valueOf());
                }
            }
        ]);
});