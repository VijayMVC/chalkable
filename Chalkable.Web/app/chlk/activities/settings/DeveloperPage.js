REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.settings.DeveloperSettings');
REQUIRE('chlk.activities.settings.SchoolPersonPage');

NAMESPACE('chlk.activities.settings', function () {

    /** @class chlk.activities.settings.DeveloperPage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [chlk.activities.lib.PageClass('profile')],
        [ria.mvc.TemplateBind(chlk.templates.settings.DeveloperSettings)],
        'DeveloperPage', EXTENDS(chlk.activities.settings.SchoolPersonPage), [ ]);
});