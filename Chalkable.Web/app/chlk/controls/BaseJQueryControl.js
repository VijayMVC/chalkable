REQUIRE('chlk.controls.Base');

NAMESPACE('chlk.controls', function () {

    /** @class chlk.controls.BaseJQueryControl*/
    CLASS(
        'BaseJQueryControl', EXTENDS(chlk.controls.Base), [

            [[Object]],
            VOID, function update(node){

            },

            [[Object]],
            Object, function processAttrs(attributes) {
                attributes.id = attributes.id || ria.dom.Dom.GID();
                this.context.getDefaultView()
                    .onActivityRefreshed(function (activity, model) {
                        this.update(jQuery('#'+attributes.id));
                    }.bind(this));
                return attributes;
            }

        ]);
});