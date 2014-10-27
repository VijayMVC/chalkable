REQUIRE('chlk.activities.lib.TemplateDialog');
REQUIRE('chlk.templates.grading.StudentAvgPopUpTpl');

NAMESPACE('chlk.activities.grading', function(){

    /**@class chlk.activities.grading.StudentAvgPopupDialog*/
    CLASS(
        [ria.mvc.DomAppendTo('#chlk-dialogs')],
        [ria.mvc.ActivityGroup('ReportDialog')],
        [ria.mvc.TemplateBind(chlk.templates.grading.StudentAvgPopUpTpl)],
        'StudentAvgPopupDialog', EXTENDS(chlk.activities.lib.TemplateDialog),[

            OVERRIDE, VOID, function onRender_(model){
                BASE(model);
                var that = this;
                new ria.dom.Dom('#chlk-overlay').on('click', function(){
                    that.dom.find('#avg-cancel').trigger('click');
                    return false;
                })
            }

        ]);
});