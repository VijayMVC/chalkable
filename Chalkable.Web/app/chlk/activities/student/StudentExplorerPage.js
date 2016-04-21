REQUIRE('chlk.activities.student.StudentProfileExplorerPage');
REQUIRE('chlk.templates.student.StudentExplorerTpl');
REQUIRE('chlk.templates.apps.SuggestedAppsForExplorerListTpl');

NAMESPACE('chlk.activities.student', function () {

    /** @class chlk.activities.student.StudentExplorerPage */

    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [chlk.activities.lib.PageClass('profile')],
        [ria.mvc.TemplateBind(chlk.templates.student.StudentExplorerTpl)],
        [ria.mvc.PartialUpdateRule(chlk.templates.apps.SuggestedAppsForExplorerListTpl, 'apps', '.suggested-apps-container', ria.mvc.PartialUpdateRuleActions.Replace)],
        'StudentExplorerPage', EXTENDS(chlk.activities.student.StudentProfileExplorerPage), []);
});