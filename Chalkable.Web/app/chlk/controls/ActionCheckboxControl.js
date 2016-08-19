REQUIRE('chlk.controls.Base');

NAMESPACE('chlk.controls', function () {

    /** @class chlk.controls.ActionCheckboxControl */
    CLASS(
        'ActionCheckboxControl', EXTENDS(chlk.controls.Base), [
            OVERRIDE, VOID, function onCreate_() {
                BASE();
                ASSET('~/assets/jade/controls/action-checkbox.jade')(this);
            },

            [[Object]],
            Object, function prepareData(attributes) {
                attributes.id = attributes.id || ria.dom.Dom.GID();
                this.context.getDefaultView()
                    .onActivityRefreshed(function (activity, model) {
                        this.addEvents(attributes.id, activity.getDom());
                    }.bind(this));
                return attributes;
            },

            [[String, ria.dom.Dom]],
            function addEvents(id, dom_){
                dom_ = dom_ || new ria.dom.Dom();
                var node = dom_.find('#' + id);
                node.off('change.action-check');
                node.on('change.action-check', function(node, event){
                    if(node.getAttr('readonly'))
                        return false;
                    setTimeout(function(){
                        var link = node.parent('.action-checkbox-container').find(node.checked() ? '.check-link' : '.un-check-link');
                        link.trigger('click');
                    }, 1);
                });
            }
        ]);
});