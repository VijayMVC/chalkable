REQUIRE('chlk.templates.grading.GradingClassSummaryTpl');
REQUIRE('chlk.templates.grading.TeacherClassGradingSummaryItemTpl');
REQUIRE('chlk.templates.grading.GradingClassSummaryItemTpl');
REQUIRE('chlk.activities.grading.BaseGradingPage');

NAMESPACE('chlk.activities.grading', function () {

    var gradingGridTimer;

    /** @class chlk.activities.grading.GradingClassSummaryPage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.grading.GradingClassSummaryTpl)],
        'GradingClassSummaryPage', EXTENDS(chlk.activities.grading.BaseGradingPage), [
            [ria.mvc.DomEventBind('click', '.mp-title')],
            [[ria.dom.Dom, ria.dom.Event]],
            OVERRIDE, VOID, function collapseClick(node, event){
                new ria.dom.Dom('.ann-grade-pop-up').remove();
                var nodeT = new ria.dom.Dom(event.target);
                var dom = this.dom;
                if(!nodeT.hasClass('comment-button')){
                    var parent = node.parent('.marking-period-container');

                    var mpData = parent.find('.mp-data');

                    if(parent.hasClass('open')){
                        jQuery(mpData.valueOf()).animate({
                            height: 0
                        }, 500);

                        mpData.addClass('with-data');

                        setTimeout(function(){
                            parent.removeClass('open');
                        }, 500);
                    }else{
                        var items = this.dom.find('.marking-period-container.open');
                        var itemsMp = items.find('.mp-data');
                        jQuery(itemsMp.valueOf()).animate({height: 0}, 500);
                        if(mpData.hasClass('with-data')){
                            mpData.removeClass('with-data');
                            this.openGradingPeriod(mpData);
                        }else{
                            clearTimeout(gradingGridTimer);
                            parent.find('.load-grading-period').trigger('submit');
                        }
                        dom.find('.mp-data.with-data')
                            .setHTML('')
                            .removeClass('with-data');
                        setTimeout(function(){
                            items.removeClass('open');
                            itemsMp.setHTML('');
                        }, 500);
                    }
                }
            },

            function openGradingPeriod(container){
                container.parent('.marking-period-container').addClass('open');
                var annContainer = container.find('.ann-types-container');
                container.setCss('height', 0);
                jQuery(container.valueOf()).animate({
                    height: (annContainer.height() + parseInt(annContainer.getCss('margin-bottom'), 10))
                }, 500);
            },

            [ria.mvc.PartialUpdateRule(chlk.templates.grading.TeacherClassGradingSummaryItemTpl)],
            VOID, function updateGradingPeriodPart(tpl, model, msg_) {
                var container = this.dom.find('.mp-data[data-grading-period-id=' + model.getGradingPeriod().getId().valueOf() + ']');
                var tooltipText = model.getTooltipText();
                tpl.renderTo(container.setHTML(''));
                setTimeout(function(){
                    this.openGradingPeriod(container);
                    container.parent().find('.mp-title').setData('tooltip', tooltipText);
                }.bind(this), 1);

            }

        ]);
});