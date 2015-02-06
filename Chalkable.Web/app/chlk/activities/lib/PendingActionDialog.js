REQUIRE('ria.mvc.DomActivity');

NAMESPACE('chlk.activities.lib', function () {


    /**@class chlk.activities.lib.PendingActionDialog*/
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.ActivityGroup('InfoMessage')],
        'PendingActionDialog', EXTENDS(ria.mvc.DomActivity), [
            OVERRIDE, ria.dom.Dom, function onDomCreate_() {
                return new ria.dom.Dom().fromHTML('<div class="loading-page"></div>');
            }
        ]);
});
