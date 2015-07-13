REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.setup.ClassroomOptionSetupTpl');

NAMESPACE('chlk.activities.setup', function () {

    /** @class chlk.activities.setup.ClassroomOptionSetupPage*/
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.setup.ClassroomOptionSetupTpl)],
        'ClassroomOptionSetupPage', EXTENDS(chlk.activities.lib.TemplatePage), [

        ]);
});