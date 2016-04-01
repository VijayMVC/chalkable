REQUIRE('chlk.activities.common.standards.BaseStandardDialog');
REQUIRE('chlk.templates.apps.AddTopicDialogTpl');

NAMESPACE('chlk.activities.apps', function(){

    /**@class chlk.activities.apps.AddTopicDialog*/
    CLASS(
        [ria.mvc.ActivityGroup('AttachABDialog')],
        [ria.mvc.DomAppendTo('#chlk-dialogs')],
        [ria.mvc.PartialUpdateRule(chlk.templates.standard.ItemsListTpl, 'list-update', '.browse-items-cnt', ria.mvc.PartialUpdateRuleActions.Replace)],
        [ria.mvc.PartialUpdateRule(chlk.templates.standard.StandardsBreadcrumbTpl, 'add-breadcrumb', '.breadcrumbs-cnt', ria.mvc.PartialUpdateRuleActions.Append)],
        [ria.mvc.PartialUpdateRule(chlk.templates.standard.StandardsBreadcrumbTpl, 'replace-breadcrumbs', '.breadcrumbs-cnt-main', ria.mvc.PartialUpdateRuleActions.Replace)],
        [ria.mvc.PartialUpdateRule(chlk.templates.standard.StandardsMainTableTpl, 'clear-search', '.browse-cnt', ria.mvc.PartialUpdateRuleActions.Replace)],
        [ria.mvc.TemplateBind(chlk.templates.apps.AddTopicDialogTpl)],

        'AddTopicDialog', EXTENDS(chlk.activities.common.standards.BaseStandardDialog),[]);
});