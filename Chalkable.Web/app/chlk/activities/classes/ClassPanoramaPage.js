REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.classes.ClassProfilePanoramaTpl');

NAMESPACE('chlk.activities.classes', function () {

    /** @class chlk.activities.classes.ClassPanoramaPage */

    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [chlk.activities.lib.PageClass('profile')],
        [ria.mvc.TemplateBind(chlk.templates.classes.ClassProfilePanoramaTpl)],
        'ClassPanoramaPage', EXTENDS(chlk.activities.lib.TemplatePage), [

            [ria.mvc.PartialUpdateRule(null, 'save-filters')],
            VOID, function afterFiltersSave(tpl, model, msg_) {
                console.info('refresh');
            }
        ]);
});