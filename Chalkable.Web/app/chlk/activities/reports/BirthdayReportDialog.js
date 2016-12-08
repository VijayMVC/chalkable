REQUIRE('chlk.activities.lib.TemplateDialog');
REQUIRE('chlk.templates.reports.BirthdayReportTpl');

NAMESPACE('chlk.activities.reports', function(){

    /**@class chlk.activities.reports.BirthdayReportDialog*/
    CLASS(
        [ria.mvc.DomAppendTo('#chlk-dialogs')],
        [ria.mvc.ActivityGroup('ReportDialog')],
        [ria.mvc.TemplateBind(chlk.templates.reports.BirthdayReportTpl)],
        'BirthdayReportDialog', EXTENDS(chlk.activities.lib.TemplateDialog),[
            [ria.mvc.DomEventBind('change', '#report-for')],
            [[ria.dom.Dom, ria.dom.Event, Object]],
            function reportForChange(node, event, selected_){
                if(node.getValue() == 2){
                    this.dom.find('.hasDatepicker').setAttr('disabled', 'disabled');
                    this.dom.find('.month-select').setAttr('disabled', false)
                           .trigger('chosen:updated');
                }else{
                    this.dom.find('.hasDatepicker').setAttr('disabled', false);
                    this.dom.find('.month-select').setAttr('disabled', 'disabled')
                           .trigger('chosen:updated');
                }
            }
        ]);
});