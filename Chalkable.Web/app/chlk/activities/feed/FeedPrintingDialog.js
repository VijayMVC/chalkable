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
                var checkNodes = this.dom.find('[type=checkbox][name="includehiddenattributes"], [type=checkbox][name="includeattachments"]');
                checkNodes.setValue(false);
                checkNodes.trigger(chlk.controls.CheckBoxEvents.DISABLED_STATE.valueOf(), [!value]);

                //set hidden attribute as checked if activityDetails is also checked
                //Note: By default hidden attribute should be checked
                this.dom.find('[type=checkbox][name="includehiddenattributes"]').setValue(value);

                var standardsNode = this.dom.find('[type=checkbox][name="groupbystandards"]');
                standardsNode.setValue(false);
                standardsNode.trigger(chlk.controls.CheckBoxEvents.DISABLED_STATE.valueOf(), [value]);
            }
        ]);
});