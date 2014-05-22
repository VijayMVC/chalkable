REQUIRE('chlk.controls.Base');

NAMESPACE('chlk.controls', function () {

    /** @class chlk.controls.SlideCheckboxControl*/
    CLASS(
        'SlideCheckboxControl', EXTENDS(chlk.controls.Base), [
            OVERRIDE, VOID, function onCreate_() {
                BASE();
                ASSET('~/assets/jade/controls/slide-checkbox.jade')(this);
            },

            [[Object]],
            Object, function prepareAttributes(attributes){
                attributes.id = attributes.id || ria.dom.Dom.GID();
                return attributes;
            }
        ]);
});