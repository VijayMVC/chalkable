REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.grading.GradingStudentSummaryTpl');
REQUIRE('chlk.templates.grading.GradingStudentSummaryChartTpl');

NAMESPACE('chlk.activities.grading', function () {

    /** @class chlk.activities.grading.GradingStudentSummaryPage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.grading.GradingStudentSummaryTpl)],
        [ria.mvc.PartialUpdateRule(chlk.templates.grading.GradingStudentSummaryChartTpl, 'chart-update', '.chart-container-1', ria.mvc.PartialUpdateRuleActions.Replace)],
        'GradingStudentSummaryPage', EXTENDS(chlk.activities.lib.TemplatePage), [

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
            },
        ]);
});