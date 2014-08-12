REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.grading.GradingTeacherClassSummaryListTpl');
REQUIRE('chlk.templates.classes.TopBar');

NAMESPACE('chlk.activities.grading', function () {

    /** @class chlk.activities.grading.GradingTeacherClassSummaryPage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.grading.GradingTeacherClassSummaryListTpl)],
        'GradingTeacherClassSummaryPage', EXTENDS(chlk.activities.lib.TemplatePage), [
            [ria.mvc.DomEventBind('click', '.show-less')],
            [[ria.dom.Dom, ria.dom.Event]],
            Boolean, function showLessClick(node, event){
                node.parent('.class-block').removeClass('active');
                return false;
            },

            [ria.mvc.DomEventBind('mouseover mouseleave', '.student-item')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function studentHover(node, event){
                var id = node.getAttr('studentId');
                var pic = node.parent('.class-block').find('.student-pic-container[studentId=' + id +']');
                if(event.type == 'mouseover')
                    pic.addClass('hovered');
                else
                    pic.removeClass('hovered');
            },

            [ria.mvc.DomEventBind('mouseover mouseleave', '.student-pic-container')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function picHover(node, event){
                var id = node.getAttr('studentId');
                var student = node.parent('.class-block').find('.student-item[studentId=' + id +']');
                if(event.type == 'mouseover')
                    student.addClass('hovered');
                else
                    student.removeClass('hovered');
                student.trigger(event.type);
            },

            [ria.mvc.DomEventBind('click', '.all-button')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function allButtonClick(node, event){
                node.parent('.class-block').addClass('active');
            }
        ]);
});