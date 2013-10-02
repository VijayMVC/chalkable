REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.grading.GradingClassSummaryTpl');
REQUIRE('chlk.templates.grading.GradingClassSummaryItemTpl');

NAMESPACE('chlk.activities.grading', function () {

    /** @class chlk.activities.grading.GradingClassSummaryPage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.grading.GradingClassSummaryTpl)],
        'GradingClassSummaryPage', EXTENDS(chlk.activities.lib.TemplatePage), [
            [ria.mvc.DomEventBind('click', '.collapse')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function collapseClick(node, event){
                var parent = node.parent('.marking-period-container');
                parent.toggleClass('open');
                var mpData = parent.find('.mp-data');
                mpData.setCss('height', parent.hasClass('open') ? mpData.find('.ann-types-container').height() : 0);
            }
        ]);
});