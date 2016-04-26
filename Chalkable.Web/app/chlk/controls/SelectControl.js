REQUIRE('chlk.controls.Base');

NAMESPACE('chlk.controls', function () {

    /** @class chlk.controls.SelectControl */
    CLASS(
        'SelectControl', EXTENDS(chlk.controls.Base), [
            OVERRIDE, VOID, function onCreate_() {
                BASE();
                ASSET('~/assets/jade/controls/select.jade')(this);
            },

            [[Object, Object]],
            VOID, function updateSelect(node, attributes){
                var that = this;
                node.chosen({disable_search_threshold: attributes.maxLength || 1000}).change(function(){
                    var node = jQuery(this);
                    node.find('option[selected]').attr('selected', false);
                    var option = node.find('option[value="' + node.val() + '"]');
                    option.attr('selected', true);
                    var controller = node.data('controller');
                    if(controller){
                        var action = node.data('action');
                        var paramsArr = node.data('params') || [];
                        var params = paramsArr.slice();
                        params.unshift(node.val());
                        var additionalParams = option.data('additional-params');
                        if(additionalParams && additionalParams.length)
                            params = params.concat(additionalParams);
                        var state = that.context.getState();
                        state.setController(controller);
                        state.setAction(action);
                        state.setParams(params);
                        state.setPublic(false);
                        that.context.stateUpdated();
                    }
                });
            },



            //TODO: make base jquery control method
            [[Object]],
            Object, function processAttrs(attributes) {
                attributes.id = attributes.id || ria.dom.Dom.GID();
                this.context.getDefaultView()
                    .onActivityRefreshed(function (activity, model) {
                        this.updateSelect(jQuery('#'+attributes.id), attributes);
                    }.bind(this));
                return attributes;
            }
        ]);
});