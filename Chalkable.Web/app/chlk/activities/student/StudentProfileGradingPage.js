REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.calendar.discipline.StudentDisciplineMonthCalendarTpl');
REQUIRE('chlk.templates.student.StudentProfileGradingTpl');
REQUIRE('chlk.templates.student.StudentProfileGradingPeriodPartTpl');

NAMESPACE('chlk.activities.student', function () {

    /** @class chlk.activities.student.StudentProfileGradingPage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [chlk.activities.lib.PageClass('profile')],
        //[ria.mvc.PartialUpdateRule(chlk.templates.calendar.discipline.StudentDisciplineMonthCalendarTpl, '', '#discipline-calendar-info', ria.mvc.PartialUpdateRuleActions.Replace)],
        [ria.mvc.TemplateBind(chlk.templates.student.StudentProfileGradingTpl)],
        'StudentProfileGradingPage', EXTENDS(chlk.activities.lib.TemplatePage), [

            [ria.mvc.PartialUpdateRule(chlk.templates.student.StudentProfileGradingPeriodPartTpl)],
            VOID, function updateGradingPeriod(tpl, model, msg_) {
                var container = this.dom.find('.gp-items[data-id=' + model.getGradingPeriod().getId().valueOf() + ']');
                tpl.renderTo(container.setHTML(''));

                var node = this.dom.find('.gp-container.opened').find('.gp-items');
                if(node.getData('id') != model.getGradingPeriod().getId().valueOf()){
                    node.parent('.gp-container').removeClass('opened');
                    setTimeout(function(){
                        node.find('.announcement-types-info').setCss('height', 0);
                    }, 1);
                }

                container.parent('.gp-container').addClass('opened');
                setTimeout(function(){
                    var height = 0;
                    container.find('.announcement-types-info-2').forEach(function(node){
                        if(node.height() > height)
                            height = node.height()
                    });
                    container.find('.announcement-types-info').setCss('height', height);
                }, 1);

            },

            [ria.mvc.DomEventBind('click', '.gp-container.opened .gp-link')],
            [[ria.dom.Dom, ria.dom.Event]],
            Boolean, function gpClick(node, event){
                node.find('.announcement-types-info').setCss('height', 0);
                return false;
            }
        ]);
});