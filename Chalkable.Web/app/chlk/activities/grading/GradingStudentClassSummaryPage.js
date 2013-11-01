REQUIRE('chlk.activities.grading.BaseGradingPage');
REQUIRE('chlk.templates.grading.GradingStudentClassSummaryTpl');
REQUIRE('chlk.templates.classes.TopBar');

NAMESPACE('chlk.activities.grading', function () {

    /** @class chlk.activities.grading.GradingStudentClassSummaryPage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.grading.GradingStudentClassSummaryTpl)],
        'GradingStudentClassSummaryPage', EXTENDS(chlk.activities.grading.BaseGradingPage), [
            [ria.mvc.DomEventBind('mouseover mouseleave', '.item-type:not(.hovered)')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function itemTypeHover(node, event){
                var needOpacity = event.type == 'mouseleave';
                this.changeLineOpacity(node, needOpacity);
            },

            [ria.mvc.DomEventBind('click', '.item-type:not(.avg)')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function itemTypeClick(node, event){
                var needOpacity = node.hasClass('hovered');
                if(needOpacity)
                    node.removeClass('hovered');
                else
                    node.addClass('hovered');
                //this.changeLineOpacity(node, needOpacity);
            },

            [[ria.dom.Dom, Boolean]],
            VOID, function changeLineOpacity(node, needOpacity){
                var color = needOpacity ? 'rgba(193,193,193,0.2)' /*node.getData('opacity-color')*/ : node.getData('color');
                var index = node.getData('index');
                var chart = this.dom.find('.main-chart').getData('chart');
                chart.series[index].graph.attr({ stroke: color });
            }
        ]);
});