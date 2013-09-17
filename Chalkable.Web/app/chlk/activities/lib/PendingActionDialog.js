REQUIRE('ria.mvc.DomActivity');

NAMESPACE('chlk.activities.lib', function () {


    /**@class chlk.activities.lib.PendingActionDialog*/
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        'PendingActionDialog', EXTENDS(ria.mvc.DomActivity), [
            OVERRIDE, ria.dom.Dom, function onDomCreate_() {
                return new ria.dom.Dom().fromHTML('<div class="pending-action"><div class="horizontal-loader"></div></div>');
            }
        ]);
});
