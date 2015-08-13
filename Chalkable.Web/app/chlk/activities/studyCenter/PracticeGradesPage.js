REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.studyCenter.PracticeGradesTpl');

NAMESPACE('chlk.activities.studyCenter', function () {

    /** @class chlk.activities.studyCenter.PracticeGradesPage */

    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [chlk.activities.lib.PageClass('profile')],
        [ria.mvc.TemplateBind(chlk.templates.studyCenter.PracticeGradesTpl)],
        'PracticeGradesPage', EXTENDS(chlk.activities.lib.TemplatePage), [
            [ria.mvc.DomEventBind('change', '.standards-select')],
            [[ria.dom.Dom, ria.dom.Event, Object]],
            function gradingSelectChange(node, event, selected_){
                node.parent('form').trigger('submit');
            }
        ]);
});