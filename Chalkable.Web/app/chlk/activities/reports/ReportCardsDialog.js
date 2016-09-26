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
            [ria.mvc.DomEventBind('change', '.recipient-search')],
            [[ria.dom.Dom, ria.dom.Event, Object]],
            VOID, function searchChange(node, event, selected_){
                if(this.dom.find('[name=reportRecipient][type=hidden]').getValue())
                    this.dom.find('.recipient-submit').trigger('click');
            },

            [ria.mvc.DomEventBind('click', '.recipients-list')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function recipientsClick(node, event){
                if(!ria.dom.Dom(event.target).is('a') && !ria.dom.Dom(event.target).is('input'))
                    node.find('.recipient-search').trigger('focus');
            },

            [ria.mvc.DomEventBind('click', '.remove-recipient')],
            [[ria.dom.Dom, ria.dom.Event]],
            function onRemoveRecipientClick(node, event) {
                var selectedNode = this.dom.find('.selected-values'),
                    selectedO = JSON.parse(selectedNode.getValue()),
                    isGroup = node.hasClass('remove-group'),
                    id = node.getData('id'),
                    idsNode = this.dom.find(isGroup ? '#group-ids-value' : '#student-ids-value'),
                    ids = idsNode.getValue().split(',');

                ids.splice(ids.indexOf(id), 1);

                if(isGroup)
                    selectedO.groups = selectedO.groups.filter(function(group){return group.id != id});
                else
                    selectedO.students = selectedO.students.filter(function(student){return student.id != id});

                idsNode.setValue(ids.join(','));
                selectedNode.setValue(JSON.stringify(selectedO));
                node.parent('.recipient-item').removeSelf();
            },

            [ria.mvc.DomEventBind('chosen:showing_dropdown', '.custom-template-select')],
            [[ria.dom.Dom, ria.dom.Event, Object]],
            function customTemplateSelect(node, event_, b_){
                var chosenC = node.$.find('+DIV.chosen-container');
                node.$.find('option').each(function(index){
                    var icon = $(this).data('icon'),
                        img = '<img src="' + icon + '"/>';

                    chosenC.find('.active-result:eq(' + index + ')').prepend(img);
                });
            }
        ]);
});