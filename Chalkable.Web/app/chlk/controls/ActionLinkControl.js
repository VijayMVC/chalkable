REQUIRE('chlk.controls.Base');

NAMESPACE('chlk.controls', function () {

    var lastClickedNode = null;

    var lastClickedNodeSelector = null;

    chlk.controls.getActionLinkControlLastNode = function () {
        if(!lastClickedNode.exists() || !lastClickedNode.is(':visible'))
            lastClickedNode = new ria.dom.Dom(lastClickedNodeSelector);
        return lastClickedNode;
    };

    function getParsedApiVersion_(sisApiVersion){
        return sisApiVersion.split('.').map(function(item){return parseInt(item, 10)});
    }

    function isApiSupported_(sisApiVersion, linkApiVersion){
        var linkApiVersionArr = getParsedApiVersion_(linkApiVersion);
        var sisApiVersionArr = getParsedApiVersion_(sisApiVersion);
        for(var i = 0; i < sisApiVersionArr.length; i++){
            if(sisApiVersionArr[i] == linkApiVersionArr[i]) continue;
            return sisApiVersionArr[i] > linkApiVersionArr[i];
        }
        return true;
    }

    function s_ (x) {
        if (x === undefined || x === null)
            return null;

        if (Array.isArray(x))
            return x.map(s_);

        if (x.hasOwnProperty("valueOf"))
            return s_(x.valueOf());

        return x;
    }

    function s (x) {
        if (x === undefined || x === null)
            return "null";

        if (Array.isArray(x))
            return JSON.stringify(x.map(s_));

        if (x.hasOwnProperty("valueOf"))
            x = s_(x.valueOf());

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

            [[Object]],
            Object, function prepareAttributes(attrs) {
                var linkApiVersion = attrs['data-sis-api-version'];

                if(linkApiVersion){
                    var sisApiVersion = this.getContext().getSession().get(ChlkSessionConstants.SIS_API_VERSION, null);
                    var supported = isApiSupported_(sisApiVersion, linkApiVersion);

                    attrs['class'] = attrs['class'] || [];
                    if (!supported){
                        attrs['class'].push('disabled');
                        attrs['class'].push('with-events');
                        attrs['data-tooltip'] = 'Your InformationNow doesn\' support current API. This API requires InformationNow version ' + linkApiVersion + ' or later';
                    }
                }

                return attrs;
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
                return '#' + res.map(encodeURIComponent).join('/');
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

                var disabledMessage = node.getData('disabled-message'), controller, action, args;
                if(disabledMessage){
                    controller = 'error';
                    action = 'disabledLinkMsg';
                    args = [disabledMessage];
                }else{
                    var link = node.getData('link');
                    args = this.parseLink_(link);
                    controller = args.shift();
                    action = args.shift();
                }

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
