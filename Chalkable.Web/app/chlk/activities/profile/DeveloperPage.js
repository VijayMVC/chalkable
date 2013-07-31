REQUIRE('ria.mvc.TemplateActivity');
REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.profile.DeveloperProfile');

NAMESPACE('chlk.activities.profile', function () {

    /** @class chlk.activities.profile.DeveloperPage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [chlk.activities.lib.PageClass('profile')],
        [ria.mvc.TemplateBind(chlk.templates.profile.DeveloperProfile)],
        'DeveloperPage', EXTENDS(chlk.activities.lib.TemplatePage), [ ]);
});