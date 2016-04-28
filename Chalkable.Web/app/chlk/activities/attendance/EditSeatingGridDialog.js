REQUIRE('chlk.activities.lib.TemplateDialog');
REQUIRE('chlk.templates.attendance.EditSeatingGridTpl');

NAMESPACE('chlk.activities.attendance', function(){

    /**@class chlk.activities.attendance.EditSeatingGridDialog*/
    CLASS(
        [ria.mvc.DomAppendTo('#chlk-dialogs')],
        [ria.mvc.TemplateBind(chlk.templates.attendance.EditSeatingGridTpl)],
        'EditSeatingGridDialog', EXTENDS(chlk.activities.lib.TemplateDialog),[]);
});