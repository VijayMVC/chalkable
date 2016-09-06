REQUIRE('chlk.activities.lib.TemplateDialog');
REQUIRE('chlk.templates.reports.ReportCardsSubmitFormTpl');
REQUIRE('chlk.templates.reports.ReportCardRecipientsTpl');

NAMESPACE('chlk.activities.reports', function(){

    /**@class chlk.activities.reports.ReportCardsDialog*/
    CLASS(
        [ria.mvc.DomAppendTo('#chlk-dialogs')],
        [ria.mvc.ActivityGroup('ReportDialog')],
        [ria.mvc.TemplateBind(chlk.templates.reports.ReportCardsSubmitFormTpl)],
        [ria.mvc.PartialUpdateRule(chlk.templates.reports.ReportCardRecipientsTpl, 'recipients', '.recipients-list', ria.mvc.PartialUpdateRuleActions.Replace)],
        'ReportCardsDialog', EXTENDS(chlk.activities.lib.TemplateDialog),[
            [ria.mvc.DomEventBind('click', '.remove-recipient')],
            [[ria.dom.Dom, ria.dom.Event]],
            function onRemoveRecipientClick(node, event) {
                var id = node.getData('id'),
                    idsNode = this.dom.find('#group-ids-value'),
                    ids = idsNode.getValue().split(',');

                ids.splice(ids.indexOf(id), 1);
                idsNode.setValue(ids.join(','));
                node.parent('.grey-button').removeSelf();
            },

            [ria.mvc.DomEventBind('chosen:showing_dropdown', '.custom-template-select')],
            [[ria.dom.Dom, ria.dom.Event, Object]],
            function customTemplateSelect(node, event_, b_){
                console.info(node);
                var chosenC = node.$.find('+DIV.chosen-container');
                node.$.find('option').each(function(index){
                    var icon = $(this).data('icon'),
                        img = '<img src="' + icon + '"/>';

                    chosenC.find('.active-result:eq(' + index + ')').prepend(img);
                });
            }
        ]);
});