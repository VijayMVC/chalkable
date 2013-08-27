REQUIRE('chlk.controls.Base');

NAMESPACE('chlk.controls', function () {
    /** @class chlk.controls.SearchBoxControl */
    CLASS(
        'SearchBoxControl', EXTENDS(chlk.controls.Base), [
            OVERRIDE, VOID, function onCreate_() {
                BASE();
                ASSET('~/assets/jade/controls/SearchBox.jade')(this);
            },

            [ria.mvc.DomEventBind('keyup', 'input[search-box]')],
            [[ria.dom.Dom, ria.dom.Event]],
            Boolean, function onActionLinkClick(node, event) {
                console.log(node.getValue());
            }

        ]);
});
