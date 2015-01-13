REQUIRE('chlk.activities.common.standards.BaseStandardDialog');
REQUIRE('chlk.templates.announcement.AddStandardsTpl');
REQUIRE('chlk.templates.standard.StandardsListTpl');

NAMESPACE('chlk.activities.announcement', function(){

    /**@class chlk.activities.announcement.AddStandardsDialog*/
    CLASS(
        [ria.mvc.DomAppendTo('#chlk-dialogs')],
        [ria.mvc.PartialUpdateRule(chlk.templates.standard.StandardsListTpl, '', '.standards-row', ria.mvc.PartialUpdateRuleActions.Append)],
        [ria.mvc.TemplateBind(chlk.templates.announcement.AddStandardsTpl)],

        'AddStandardsDialog', EXTENDS(chlk.activities.common.standards.BaseStandardDialog),[]);
});