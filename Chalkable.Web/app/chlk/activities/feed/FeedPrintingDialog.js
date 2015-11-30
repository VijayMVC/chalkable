REQUIRE('chlk.activities.lib.TemplateDialog');
REQUIRE('chlk.templates.reports.FeePrintingReportTpl');

NAMESPACE('chlk.activities.feed', function(){

    /**@class chlk.activities.feed.FeedPrintingDialog*/
    CLASS(
        [ria.mvc.DomAppendTo('#chlk-dialogs')],
        [ria.mvc.ActivityGroup('ReportDialog')],
        [ria.mvc.TemplateBind(chlk.templates.reports.FeePrintingReportTpl)],
        'FeedPrintingDialog', EXTENDS(chlk.activities.lib.TemplateDialog),[

            [ria.mvc.DomEventBind('change', '[name="includedetails"]')],
            [[ria.dom.Dom, ria.dom.Event]],
            function includeDetailsChange(node, event){
                var value = JSON.parse(node.getValue());
                var nodes = this.dom.find('[name="includehiddenattributes"], [name="includeattachments"]');
                nodes.parent().toggleClass('disabled');
                nodes.setAttr('disabled', value ? false : 'disabled');
                nodes.setProp('disabled', value ? false : true);
            }
        ]);
});