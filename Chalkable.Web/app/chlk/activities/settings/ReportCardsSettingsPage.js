REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.settings.ReportCardsSettingsTpl');

NAMESPACE('chlk.activities.settings', function(){

    /**@class chlk.activities.settings.ReportCardsSettingsPage*/
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [chlk.activities.lib.PageClass('profile')],
        [ria.mvc.TemplateBind(chlk.templates.settings.ReportCardsSettingsTpl)],
        'ReportCardsSettingsPage', EXTENDS(chlk.activities.lib.TemplatePage),[]);
});