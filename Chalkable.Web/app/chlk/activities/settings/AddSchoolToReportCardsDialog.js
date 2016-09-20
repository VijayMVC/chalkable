REQUIRE('chlk.templates.settings.AddSchoolToReportCardsTpl');
REQUIRE('chlk.activities.lib.TemplateDialog');

NAMESPACE('chlk.activities.settings', function(){

    /**@class chlk.activities.settings.AddSchoolToReportCardsDialog*/
    CLASS(
        [ria.mvc.DomAppendTo('#chlk-dialogs')],
        [ria.mvc.TemplateBind(chlk.templates.settings.AddSchoolToReportCardsTpl)],
        'AddSchoolToReportCardsDialog', EXTENDS(chlk.activities.lib.TemplateDialog),[
            [ria.mvc.DomEventBind('submit', 'form')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function formSubmit(node, event){
                var res=[];
                this.dom.find('.school-select').find(':selected').forEach(function(node){res.push(node.getText())});
                this.dom.find('.school-names').setValue(res.join(','));
            }
        ]);
});