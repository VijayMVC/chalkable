REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.apps.AppGeneral');

NAMESPACE('chlk.activities.apps', function () {

    /** @class chlk.activities.apps.AppGeneralInfoPage*/
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.apps.AppGeneral)],
        'AppGeneralInfoPage', EXTENDS(chlk.activities.lib.TemplatePage), [ ]);
});