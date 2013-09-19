REQUIRE('chlk.controls.Base');


NAMESPACE('chlk.controls', function () {

    /** @class chlk.controls.AppsListControl*/
    CLASS(
        'AppsListControl', EXTENDS(chlk.controls.Base), [
            OVERRIDE, VOID, function onCreate_() {
                BASE();
                ASSET('~/assets/jade/controls/apps-list.jade')(this);
            },

            [[Object]],
            VOID, function update(node){
                var that = this;
                node.chosen({disable_search_threshold: 1000}).change(function(){
                    var node = jQuery(this);
                    var selected = node.find('option:selected');
                    var controller = selected.data('controller');

                    node.find('option[selected]').attr('selected', false);

                    if(controller){
                        var action = selected.data('action');
                        var params = selected.data('params') || [];
                        var state = that.context.getState();
                        state.setController(controller);
                        state.setAction(action);
                        state.setParams(params);
                        state.setPublic(false);
                        that.context.stateUpdated();
                    }

                });
            },



            //TODO: move to base control
            [[Object]],
            Object, function processAttrs(attributes) {
                attributes.id = attributes.id || ria.dom.NewGID();
                this.context.getDefaultView()
                    .onActivityRefreshed(function (activity, model) {
                        this.update(jQuery('#'+attributes.id));
                    }.bind(this));
                return attributes;
            }
        ]);
});