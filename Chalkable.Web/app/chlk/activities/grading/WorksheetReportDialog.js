REQUIRE('chlk.activities.lib.TemplateDialog');
REQUIRE('chlk.templates.grading.WorksheetReportTpl');
REQUIRE('chlk.templates.grading.WorksheetReportGridTpl');

NAMESPACE('chlk.activities.grading', function(){

    /**@class chlk.activities.grading.WorksheetReportDialog*/
    CLASS(
        [ria.mvc.DomAppendTo('#chlk-dialogs')],
        [ria.mvc.ActivityGroup('ReportDialog')],
        [ria.mvc.TemplateBind(chlk.templates.grading.WorksheetReportTpl)],
        [ria.mvc.PartialUpdateRule(chlk.templates.grading.WorksheetReportGridTpl, 'grid', '.grid-container', ria.mvc.PartialUpdateRuleActions.Replace)],
        'WorksheetReportDialog', EXTENDS(chlk.activities.lib.TemplateDialog),[
            [ria.mvc.DomEventBind('change', '.report-date-picker')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function reportDatePickerChange(node, event){
                node.parent('form').trigger('submit');
            },

            [ria.mvc.DomEventBind('change', '.all-checkboxes')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function allAnnouncementsChange(node, event){
                node.parent('form')
                    .find('.grid-container')
                    .find('.row:not(.header)')
                    .find('[type=checkbox]')
                    .forEach(function(item){
                        if(item.checked() != node.checked())
                            item.trigger('click');
                    });
            },

            [ria.mvc.DomEventBind('submit', 'form')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function formSubmit(node, event){
                var res = [];
                node.find('.grid-container')
                    .find('.row:not(.header)')
                    .find('input[type=checkbox]').forEach(function(item){
                        if(item.checked()){
                            res.push(item.parent('.cell').getData('id'));
                        }
                });
                node.find('[name=announcementIds]').setValue(res.join(','));
            }
        ]);
});