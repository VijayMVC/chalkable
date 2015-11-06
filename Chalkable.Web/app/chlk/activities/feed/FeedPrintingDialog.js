REQUIRE('chlk.activities.lib.TemplateDialog');
REQUIRE('chlk.templates.reports.FeePrintingReportTpl');

NAMESPACE('chlk.activities.feed', function(){

    /**@class chlk.activities.feed.FeedPrintingDialog*/
    CLASS(
        [ria.mvc.DomAppendTo('#chlk-dialogs')],
        [ria.mvc.ActivityGroup('ReportDialog')],
        [ria.mvc.TemplateBind(chlk.templates.reports.FeePrintingReportTpl)],
        'FeedPrintingDialog', EXTENDS(chlk.activities.lib.TemplateDialog),[


        ]);
});