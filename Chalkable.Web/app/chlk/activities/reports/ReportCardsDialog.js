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
                var value = this.dom.find('[name=reportRecipient][type=hidden]').getValue();
                if(value){
                    var recipientsNode = this.dom.find('.recipients-to-add'),
                        selected = recipientsNode.getValue();
                    selected = selected ? selected.split(',') : [];
                    selected.push(value);
                    recipientsNode.setValue(selected.join(','));

                    var arr = value.split('|'),
                        id = parseInt(arr[0], 10) || arr[0],
                        type = parseInt(arr[1], 10),
                        isPerson = type == chlk.models.search.SearchTypeEnum.PERSONS.valueOf(),
                        cls = isPerson ? 'student-recipient' : 'group-recipient';

                    if(!node.parent().find('.' + cls + '[data-id=' + id + ']').exists()){
                        var recipientHTML = '<div class="recipient-item ' + cls + '" data-id="' + id + '">' + node.getValue() + '<a class="remove-recipient"></a></div>';

                        new ria.dom.Dom()
                            .fromHTML(recipientHTML)
                            .insertBefore(node);

                        new ria.dom.Dom('.recipients-search').find('li[data-value^="' + id + '|' + type + '"]').addClass('disabled');
                    }
                }
            },

            [ria.mvc.DomEventBind('autocomplete-close', '.recipient-search')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function searchClose(node, event){
                if(this.dom.find('.recipients-to-add').getValue())
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