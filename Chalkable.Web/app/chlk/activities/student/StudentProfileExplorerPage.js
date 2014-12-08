REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.student.StudentProfileExplorerTpl');

NAMESPACE('chlk.activities.student', function () {

    /** @class chlk.activities.student.StudentProfileExplorerPage */

    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [chlk.activities.lib.PageClass('profile')],
        [ria.mvc.TemplateBind(chlk.templates.student.StudentProfileExplorerTpl)],
        'StudentProfileExplorerPage', EXTENDS(chlk.activities.lib.TemplatePage), [

        ]);
});