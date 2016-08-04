REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.settings.Preferences');

NAMESPACE('chlk.activities.settings', function () {

    /** @class chlk.activities.settings.PreferencesPage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [chlk.activities.lib.PageClass('settings')],
        [ria.mvc.TemplateBind(chlk.templates.settings.Preferences)],
        'PreferencesPage', EXTENDS(chlk.activities.lib.TemplatePage), [ ]);
});