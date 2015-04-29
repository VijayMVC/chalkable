REQUIRE('chlk.activities.reports.BaseReportWithStudentsDialog');
REQUIRE('chlk.templates.reports.LessonPlanReportTpl');

NAMESPACE('chlk.activities.reports', function(){

    /**@class chlk.activities.reports.LessonPlanReportDialog*/
    CLASS(
        [ria.mvc.DomAppendTo('#chlk-dialogs')],
        [ria.mvc.ActivityGroup('ReportDialog')],
        [ria.mvc.TemplateBind(chlk.templates.reports.LessonPlanReportTpl)],
        'LessonPlanReportDialog', EXTENDS(chlk.activities.reports.BaseReportWithStudentsDialog),[

            [ria.mvc.DomEventBind('submit', '.lesson-plan-report-form')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function formSubmit(node, event){
                var gpNode = node.find('#activity-categories'),
                    saNode = node.find('#activity-attributes'),
                    gpArray = node.find('#activity-categories-select').getValue(),
                    saArray = node.find('#activity-attributes-select').getValue();
                if(gpArray && gpArray.length)
                    gpNode.setValue(gpArray.join(','));
                if(saArray && saArray.length)
                    saNode.setValue(saArray.join(','));
            },

            [ria.mvc.DomEventBind('change', '#include-activities')],
            [[ria.dom.Dom, ria.dom.Event, Object]],
            VOID, function droppedChange(node, event, options_){
                if(node.checked())
                    this.dom.find('.disable-by-activities').setAttr('disabled', false)
                           .trigger('liszt:updated');
                else
                    this.dom.find('.disable-by-activities').setAttr('disabled', true)
                           .trigger('liszt:updated');
            }
        ]);
});