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
                    this.dom.find('.report-title').getValue() != "Lunch Count" ||
                    this.dom.find('.start-date').getValue() ||
                    this.dom.find('.end-date').getValue() ||
                    parseInt(this.dom.find('[name=orderBy]:checked').getValue(), 10) != chlk.models.reports.LunchCountOrderBy.STUDENT.valueOf() ||
                    parseInt(this.dom.find('[name=idToPrint]:checked').getValue(), 10) != this.getIdToPrint().valueOf() ||
                    includeArray.length > 0
                )
                    return this.view.ShowLeaveConfirmBox();

                return true;
            }
        ]);
});