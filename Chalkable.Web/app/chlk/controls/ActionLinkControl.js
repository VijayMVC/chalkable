REQUIRE('chlk.controls.Base');

NAMESPACE('chlk.controls', function () {

    var lastClickedNode = null;

    var lastClickedNodeSelector = null;

    chlk.controls.getActionLinkControlLastNode = function () {
        if(!lastClickedNode.exists() || !lastClickedNode.is(':visible'))
            lastClickedNode = new ria.dom.Dom(lastClickedNodeSelector);
        return lastClickedNode;
    };

    function s (x) {
        if (x === undefined || x === null)
            return "null";

        if (Array.isArray(x))
            return JSON.stringify(x.map(s));

        if (x.hasOwnProperty("valueOf"))
            return s(x.valueOf());

        return JSON.stringify(x);
    }

    /** @class chlk.controls.ActionLinkControl */
    CLASS(
        'ActionLinkControl', EXTENDS(chlk.controls.Base), [
            OVERRIDE, VOID, function onCreate_() {
                BASE();
                ASSET('~/assets/jade/controls/action-link.jade')(this);
            },

            [[Array]],
            String, function getLink(values) {
                return encodeURIComponent(values.map(s).join(','));
            },

            [[Array]],
            String, function parseValues_(values) {
                var res = [];
                values.forEach(function(item){
                    item = item && item.valueOf ? item.valueOf() : item;
                    if(Array.isArray(item) || typeof item == "number" || typeof item == "string")
                        res.push(item);
                    else
                        res.push(JSON.stringify(item));
                });
                return res.join('/');
            },

            [[Array]],
            String, function getHref(values) {
                var res = [], that = this;
                values.forEach(function(item){
                    item = item && item.valueOf ? item.valueOf() : item;
                    if(Array.isArray(item))
                        res.push(that.parseValues_(item));
                    else
                        if(typeof item == "number" || typeof item == "string")
                            res.push(item);
                        else
                            res.push(JSON.stringify(item));
                });
                return '#' + res.join('/');
            },

            [[String]],
            Array, function parseLink_(link) {
                return JSON.parse(String('[' + decodeURIComponent(link) + ']'));
            },

            [ria.mvc.DomEventBind('click', 'A[data-link]:not(.disabled, .pressed, [disabled])')],
            [[ria.dom.Dom, ria.dom.Event]],
            Boolean, function onActionLinkClick(node, event) {
                lastClickedNode = node;
                lastClickedNodeSelector = node.getSelector();

                var link = node.getData('link');
                var args = this.parseLink_(link);
                var controller = args.shift(),
                    action = args.shift();

                if(node.hasClass('defer')){
                    setTimeout(function(){
                        this.updateState(controller, action, args);
                    }.bind(this),10)
                }else{
                    this.updateState(controller, action, args);
                }

                event.preventDefault();
                return false;
            },

            [ria.mvc.DomEventBind('click', 'A[data-link].disabled, A[data-link].pressed, A[data-link][disabled]')],
            [[ria.dom.Dom, ria.dom.Event]],
            Boolean, function onDisabledActionLinkClick(node, event) {
                return false;
            },


            [ria.mvc.DomEventBind('click', 'span.chlk-button.action-button:not(.disabled)')],
            [[ria.dom.Dom, ria.dom.Event]],
            Boolean, function onActionLinkButtonClick(node, event) {
                return this.onActionLinkClick(node.find('a'), event);
            },

            [ria.mvc.DomEventBind('click', 'BUTTON.action-button:not(.disabled)')],
            [[ria.dom.Dom, ria.dom.Event]],
            Boolean, function onActionButtonClick(node, event) {
                return this.onActionLinkClick(node, event);
            },

            VOID, function updateState(controller, action, args){
                var state = this.context.getState();
                state.setController(controller);
                state.setAction(action);
                state.setParams(args);
                state.setPublic(false);
                this.context.stateUpdated();
            }
        ]);
});