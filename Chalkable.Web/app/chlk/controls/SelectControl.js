REQUIRE('chlk.controls.Base');

NAMESPACE('chlk.controls', function () {

    /** @class chlk.controls.SelectControl */
    CLASS(
        'SelectControl', EXTENDS(chlk.controls.Base), [
            OVERRIDE, VOID, function onCreate_() {
                BASE();
                ASSET('~/assets/jade/controls/select.jade')(this);
            },

            [[Object]],
            Object, function processAttrs(attributes) {
                attributes.id = attributes.id || ria.dom.NewGID();
                this.context.getDefaultView()
                    .onActivityRefreshed(function (activity, model) {
                        var that = this;
                        jQuery('#' + attributes.id).chosen({disable_search_threshold: 1000}).change(function(){
                            var node = jQuery(this);
                            var controller = node.data('controller');
                            if(controller){
                                var action = node.data('action');
                                var params = node.data('params') || [];
                                params.unshift(node.val());
                                var state = that.context.getState();
                                state.setController(controller);
                                state.setAction(action);
                                state.setParams(params);
                                state.setPublic(false);
                                that.context.stateUpdated();
                            }
                        });
                    }.bind(this));
                return attributes;
            }
        ]);
});