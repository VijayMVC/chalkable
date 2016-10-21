REQUIRE('chlk.activities.lib.TemplateDialog');
REQUIRE('chlk.templates.reports.LunchCountSubmitFormTpl');
REQUIRE('chlk.templates.reports.ReportCardRecipientsTpl');

NAMESPACE('chlk.activities.reports', function(){

    /**@class chlk.activities.reports.LunchCountDialog*/
    CLASS(
        [ria.mvc.DomAppendTo('#chlk-dialogs')],
        [ria.mvc.ActivityGroup('ReportDialog')],
        [ria.mvc.TemplateBind(chlk.templates.reports.LunchCountSubmitFormTpl)],
        [ria.mvc.PartialUpdateRule(chlk.templates.reports.ReportCardRecipientsTpl, 'recipients', '.recipients-list', ria.mvc.PartialUpdateRuleActions.Replace)],
        'LunchCountDialog', EXTENDS(chlk.activities.lib.TemplateDialog),[
            chlk.models.reports.StudentIdentifierEnum, 'idToPrint',

            [[Object]],
            OVERRIDE, VOID, function onRefresh_(model){
                BASE(model);
                this.setIdToPrint(model.getIdToPrint() || chlk.models.reports.StudentIdentifierEnum.NONE);
            },

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
                        var recipientHTML = '<div class="recipient-item ' + cls + '" data-id="' + id + '">' + node.getData('text-value') + '<a class="remove-recipient"></a></div>';

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

                if(!selectedO.students.length && !selectedO.groups.length)
                    this.dom.find('.recipient-search').setAttr('placeholder', "Type or select groups/students");

            },

            [ria.mvc.DomEventBind('keypress', 'input')],
            [[ria.dom.Dom, ria.dom.Event]],
            function inputKeyPress(node, event){
                if(event.keyCode == ria.dom.Keys.ENTER){
                    event.preventDefault();
                }
            },

            [ria.mvc.DomEventBind('change', '.report-type-select')],
            [[ria.dom.Dom, ria.dom.Event, Object]],
            function reportTypeChange(node, event, selected_){
                var btn = this.dom.find('.change-type-btn');
                btn.trigger('click');
                node.$.val(btn.getData('type-value'));
                node.trigger('chosen:updated');
            },

            OVERRIDE, Object, function isReadyForClosing() {
                var includeArray = this.dom.find('.include-select').getValue() || [];

                if(
                    this.dom.find('#group-ids-value').getValue() ||
                    this.dom.find('#student-ids-value').getValue() ||
                    this.dom.find('.report-title').getValue() ||
                    this.dom.find('.start-date').getValue() ||
                    this.dom.find('.end-date').getValue() ||
                    parseInt(this.dom.find('[name=orderBy]:checked').getValue(), 10) != chlk.models.reports.LunchCountOrderBy.STUDENT.valueOf() ||
                    !parseInt(this.dom.find('[name=allActiveMeals]:checked').getValue(), 10) ||
                    parseInt(this.dom.find('[name=idToPrint]:checked').getValue(), 10) != this.getIdToPrint().valueOf() ||
                    includeArray.length > 0
                )
                    return this.view.ShowLeaveConfirmBox();

                return true;
            }
        ]);
});