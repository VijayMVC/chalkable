REQUIRE('chlk.controls.Base');

NAMESPACE('chlk.controls', function () {

    /** @class chlk.controls.LabeledCheckboxControl*/
    CLASS(
        'LabeledCheckboxControl', EXTENDS(chlk.controls.Base), [
            OVERRIDE, VOID, function onCreate_() {
                BASE();
                ASSET('~/assets/jade/controls/labeled-checkbox.jade')(this);
            },

            [[Object]],
            Object, function prepareData(attributes) {
                attributes.id = attributes.id || ria.dom.Dom.GID();
                return attributes;
            },


            //[ria.mvc.DomEventBind('click', '.labeled-checkbox')],
            [[ria.dom.Dom, ria.dom.Event]],
            Boolean, function changed_($target, event) {
            }
        ]);
});