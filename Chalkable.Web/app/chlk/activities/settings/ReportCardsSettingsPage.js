REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.settings.ReportCardsSettingsTpl');

NAMESPACE('chlk.activities.settings', function(){

    /**@class chlk.activities.settings.ReportCardsSettingsPage*/
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [chlk.activities.lib.PageClass('profile')],
        [ria.mvc.TemplateBind(chlk.templates.settings.ReportCardsSettingsTpl)],
        [ria.mvc.PartialUpdateRule(chlk.templates.settings.ReportCardsSettingsTpl, '', null , ria.mvc.PartialUpdateRuleActions.Replace)],
        'ReportCardsSettingsPage', EXTENDS(chlk.activities.lib.TemplatePage),[

            [ria.mvc.DomEventBind('click', '.no-logo-delete')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function noLogoDelete(node, event){
                node.parent('.btn-cnt').removeSelf();
            }
        ]);
});