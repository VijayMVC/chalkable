REQUIRE('chlk.activities.common.standards.BaseStandardDialog');
REQUIRE('chlk.templates.apps.AddCCStandardsTpl');
REQUIRE('chlk.templates.standard.CCStandardListTpl');

NAMESPACE('chlk.activities.apps', function(){

    /**@class chlk.activities.apps.AddCCStandardDialog*/
    CLASS(
        [ria.mvc.DomAppendTo('#chlk-dialogs')],
        [ria.mvc.PartialUpdateRule(chlk.templates.standard.CCStandardListTpl, '', '.standards-row', ria.mvc.PartialUpdateRuleActions.Append)],
        [ria.mvc.TemplateBind(chlk.templates.apps.AddCCStandardsTpl)],

        'AddCCStandardDialog', EXTENDS(chlk.activities.common.standards.BaseStandardDialog),[]);
});