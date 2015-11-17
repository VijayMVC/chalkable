REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.district.DistrictSummaryTpl');
REQUIRE('chlk.templates.district.BaseStatisticTpl');

NAMESPACE('chlk.activities.district', function () {
    var filterTimeout;

    /** @class chlk.activities.district.DistrictSummaryPage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.district.DistrictSummaryTpl)],
        [ria.mvc.PartialUpdateRule(chlk.templates.district.BaseStatisticTpl, '', '.grid-container', ria.mvc.PartialUpdateRuleActions.Replace)],
        'DistrictSummaryPage', EXTENDS(chlk.activities.lib.TemplatePage), [

            [ria.mvc.DomEventBind('keyup change', '.statistic-filter')],
            [[ria.dom.Dom, ria.dom.Event]],
            function filterKeyUp(node, event){
                clearTimeout(filterTimeout);
                var time = event.type == 'change' ? 1 : 1000;

                filterTimeout = setTimeout(function(){
                    var val = node.getValue();
                    if(val != node.getData('value')){
                        node.parent('form').trigger('submit');
                        node.setData('value', val);
                    }

                }, time);
            }
        ]);
});