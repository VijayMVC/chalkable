REQUIRE('chlk.controls.Base');

NAMESPACE('chlk.controls', function () {

    /** @class chlk.controls.ActionLinkControl */
    CLASS(
        'ActionLinkControl', EXTENDS(chlk.controls.Base), [
            OVERRIDE, VOID, function onCreate_() {
                BASE();
                ASSET('~/assets/jade/controls/action-link.jade')(this);
            },

            [[Array]],
            String, function getLink(args) {
                return args
                    .map(function (_) { return encodeURIComponent(_); })
                    .join(',');
            },

            [ria.mvc.DomEventBind('click', 'A.action-link[data-link]')],
            [[ria.dom.Dom, ria.dom.Event]],
            Boolean, function onActionLinkClick(node, event) {
                alert(node.getData('link'));
                return false;
            }
        ]);
});