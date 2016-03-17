REQUIRE('chlk.activities.common.standards.BaseStandardDialog');
REQUIRE('chlk.templates.apps.AddCommonCoreStandardsDialogTpl');

NAMESPACE('chlk.activities.apps', function(){

    /**@class chlk.activities.apps.AddCCStandardDialog*/
    CLASS(
        [ria.mvc.DomAppendTo('#chlk-dialogs')],
        [ria.mvc.PartialUpdateRule(chlk.templates.standard.ItemsListTpl, 'list-update', '.browse-items-cnt', ria.mvc.PartialUpdateRuleActions.Replace)],
        [ria.mvc.PartialUpdateRule(chlk.templates.standard.StandardsBreadcrumbTpl, 'add-breadcrumb', '.breadcrumbs-cnt', ria.mvc.PartialUpdateRuleActions.Append)],
        [ria.mvc.PartialUpdateRule(chlk.templates.standard.StandardsBreadcrumbTpl, 'replace-breadcrumbs', '.breadcrumbs-cnt', ria.mvc.PartialUpdateRuleActions.Replace)],
        [ria.mvc.PartialUpdateRule(chlk.templates.standard.StandardsMainTableTpl, 'clear-search', '.browse-cnt', ria.mvc.PartialUpdateRuleActions.Replace)],
        [ria.mvc.TemplateBind(chlk.templates.apps.AddCommonCoreStandardsDialogTpl)],

        'AddCCStandardDialog', EXTENDS(chlk.activities.common.standards.BaseStandardDialog),[]);
});