REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.calendar.discipline.StudentDisciplineMonthCalendarTpl');
REQUIRE('chlk.templates.student.StudentProfileGradingTpl');

NAMESPACE('chlk.activities.student', function () {

    /** @class chlk.activities.student.StudentProfileGradingPage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [chlk.activities.lib.PageClass('profile')],
        //[ria.mvc.PartialUpdateRule(chlk.templates.calendar.discipline.StudentDisciplineMonthCalendarTpl, '', '#discipline-calendar-info', ria.mvc.PartialUpdateRuleActions.Replace)],
        [ria.mvc.TemplateBind(chlk.templates.student.StudentProfileGradingTpl)],
        'StudentProfileGradingPage', EXTENDS(chlk.activities.lib.TemplatePage), [

            function $(){
                BASE();
                this._HIDDEN_CLASS = 'x-hidden';
            },

            [ria.mvc.DomEventBind('click', '.new-grading-box.item-type')],
            [[ria.dom.Dom, ria.dom.Event]],
            function showItems(node, event){
                var itemTypesNodes = this.dom.find('.grading-info').find('.item-type').valueOf();
                for(var i = 0; i < itemTypesNodes.length; i++){
                     if(itemTypesNodes[i] != node.valueOf()[0]){
                        this.hideItemTypeDetails_(new ria.dom.Dom(itemTypesNodes[i]));
                     }
                }
                node.find('.item-type-details').toggleClass(this._HIDDEN_CLASS);
            },

            VOID, function hideItemTypeDetails_(node){
                var itemTypeDetails = node.find('.item-type-details');
                if(!itemTypeDetails.hasClass(this._HIDDEN_CLASS))
                    itemTypeDetails.addClass(this._HIDDEN_CLASS);
            }

        ]);
});