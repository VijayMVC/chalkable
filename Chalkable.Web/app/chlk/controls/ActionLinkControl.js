REQUIRE('chlk.controls.Base');

NAMESPACE('chlk.controls', function () {

    var lastClickedNode = null;

    chlk.controls.getActionLinkControlLastNode = function () { return lastClickedNode; };

    /** @class chlk.controls.ActionLinkControl */
    CLASS(
        'ActionLinkControl', EXTENDS(chlk.controls.Base), [
            OVERRIDE, VOID, function onCreate_() {
                BASE();
                ASSET('~/assets/jade/controls/action-link.jade')(this);
            },

            [[Array]],
            String, function getLink(values) {
                if(!values[2] || Array.isArray(values[2]) && !values[2].length)
                    values.splice(2,1);
                return encodeURIComponent(values.map(function(_) { return Array.isArray(_) ? _.join(',') : JSON.stringify(_.valueOf ? _.valueOf() : _) }).join(','));
            },

            [[String]],
            Array, function parseLink_(link) {
                return JSON.parse(String('[' + decodeURIComponent(link) + ']'));
            },

            [ria.mvc.DomEventBind('click', 'A[data-link]:not(.disabled, .pressed)')],
            [[ria.dom.Dom, ria.dom.Event]],
            Boolean, function onActionLinkClick(node, event) {
                lastClickedNode = node;

                var link = node.getData('link');
                var args = this.parseLink_(link);
                var controller = args.shift(),
                    action = args.shift();

                var state = this.context.getState();
                state.setController(controller);
                state.setAction(action);
                state.setParams(args);
                state.setPublic(false);

                this.context.stateUpdated();

                event.preventDefault();
            }
        ]);
});