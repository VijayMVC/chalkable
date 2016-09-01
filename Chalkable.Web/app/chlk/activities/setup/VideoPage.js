REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.setup.Video');

NAMESPACE('chlk.activities.setup', function () {

    /** @class chlk.activities.setup.VideoPage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.setup.Video)],
        'VideoPage', EXTENDS(chlk.activities.lib.TemplatePage), []);
});