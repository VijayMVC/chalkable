REQUIRE('chlk.controls.BaseJQueryControl');


NAMESPACE('chlk.controls.developer', function () {

    /** @class chlk.controls.AppsListControl*/
    CLASS(
        'AppsListControl', EXTENDS(chlk.controls.BaseJQueryControl), [
            OVERRIDE, VOID, function onCreate_() {
                BASE();
                ASSET('~/assets/jade/controls/apps-list.jade')(this);
            },

            [[Object]],
            OVERRIDE, VOID, function update(node){
                BASE(node);
                var that = this;
                node.change(function(){
                    var node = jQuery(this);
                    var selected = node.find('option:selected');
                    var controller = selected.data('controller');


                    if (selected.data('action') == 'add'){
                        node.find('option').attr('selected', false);
                    }
                    else
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
            }
        ]);
});