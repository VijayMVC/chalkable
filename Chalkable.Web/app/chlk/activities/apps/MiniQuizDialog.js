REQUIRE('chlk.activities.apps.AppWrapperDialog');
REQUIRE('chlk.templates.apps.MiniQuizDialog');
REQUIRE('chlk.AppApiHost');

NAMESPACE('chlk.activities.apps', function () {

    /**
     * @class chlk.activities.apps.MiniQuizDialog
     */
    CLASS(
        [ria.mvc.ActivityGroup('MiniQuizDialog')],
        [ria.mvc.DomAppendTo('#chlk-dialogs')],
        [ria.mvc.TemplateBind(chlk.templates.apps.MiniQuizDialog)],
        'MiniQuizDialog', EXTENDS(chlk.activities.apps.AppWrapperDialog), [

        ]);
});
