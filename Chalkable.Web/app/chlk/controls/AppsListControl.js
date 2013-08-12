REQUIRE('chlk.controls.Base');

NAMESPACE('chlk.controls', function () {

    /** @class chlk.controls.AppsListControl*/
    CLASS(
        'AppsListControl', EXTENDS(chlk.controls.Base), [
            OVERRIDE, VOID, function onCreate_() {
                BASE();
                ASSET('~/assets/jade/controls/apps-list.jade')(this);
            },


            [ria.mvc.DomEventBind('click', '.app-list-combobox')],
            [[ria.dom.Dom, ria.dom.Event]],
            function onClicked($target, node){
                var selected = $target.find('option:selected');
                var controller = selected.getData('controller');
                if(controller){
                     var action = selected.getData('action');
                     var params = selected.getData('params') || [];
                     var state = this.context.getState();
                     state.setController(controller);
                     state.setAction(action);
                     state.setParams(params);
                     state.setPublic(false);
                     this.context.stateUpdated();
                }
            }
        ]);
});