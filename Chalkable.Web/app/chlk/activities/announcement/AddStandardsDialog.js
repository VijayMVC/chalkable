REQUIRE('chlk.activities.common.standards.BaseStandardDialog');

NAMESPACE('chlk.activities.announcement', function(){

    /**@class chlk.activities.announcement.AddStandardsDialog*/
    CLASS(
        [ria.mvc.ActivityGroup('AttachDialog')],
        [ria.mvc.DomAppendTo('#chlk-dialogs')],
        [ria.mvc.PartialUpdateRule(chlk.templates.standard.ItemsListTpl, 'list-update', '.browse-items-cnt', ria.mvc.PartialUpdateRuleActions.Replace)],
        [ria.mvc.PartialUpdateRule(chlk.templates.standard.StandardsBreadcrumbTpl, 'add-breadcrumb', '.breadcrumbs-cnt', ria.mvc.PartialUpdateRuleActions.Append)],
        [ria.mvc.PartialUpdateRule(chlk.templates.standard.StandardsBreadcrumbTpl, 'replace-breadcrumbs', '.breadcrumbs-cnt-main', ria.mvc.PartialUpdateRuleActions.Replace)],
        [ria.mvc.PartialUpdateRule(chlk.templates.standard.StandardsMainTableTpl, 'clear-search', '.browse-cnt', ria.mvc.PartialUpdateRuleActions.Replace)],
        [ria.mvc.TemplateBind(chlk.templates.standard.AddStandardsTpl)],

        'AddStandardsDialog', EXTENDS(chlk.activities.common.standards.BaseStandardDialog),[

        ]);
});