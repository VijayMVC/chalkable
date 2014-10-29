REQUIRE('chlk.activities.grading.BaseGradingPage');
REQUIRE('chlk.templates.grading.GradingStudentClassSummaryTpl');
REQUIRE('chlk.templates.classes.TopBar');

NAMESPACE('chlk.activities.grading', function () {

    /** @class chlk.activities.grading.GradingStudentClassSummaryPage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.grading.GradingStudentClassSummaryTpl)],
        'GradingStudentClassSummaryPage', EXTENDS(chlk.activities.grading.BaseGradingPage), [
            [ria.mvc.DomEventBind('mouseover mouseleave', '.legend-item:not(.hovered)')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function itemTypeHover(node, event){
                var needOpacity = event.type == 'mouseleave';
                this.changeLineOpacity(node, needOpacity);
            },

            [ria.mvc.DomEventBind('click', '.legend-item:not(.avg)')],
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
            },

            //todo: to many cope past
            [ria.mvc.DomEventBind('click', '.announcement-link')],
            [[ria.dom.Dom, ria.dom.Event]],
            Boolean, function announcementClick(node, event){
                var item = node.parent('.feed-item');
                var clone = item.clone();
                clone.addClass('animated-item');
                clone = clone.wrap('<div class="moving-wrapper"></div>').parent();
                clone.setCss('left', item.offset().left - 4);
                clone.setCss('top', item.offset().top);
                clone.appendTo(new ria.dom.Dom('body'));
                setTimeout(function(){
                    clone.setCss('top', 54);
                    this.dom.remove();
                    jQuery(document).scrollTop(0);
                }.bind(this), 1);
                setTimeout(function(){
                    clone.find('.announcement-link').removeClass('disabled').trigger('click');
                }, 301);
                return false;
            }
        ]);
});