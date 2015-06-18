REQUIRE('chlk.controls.CheckboxControl');

NAMESPACE('chlk.controls', function () {

    /** @class chlk.controls.SlideCheckboxControl*/
    CLASS(
        'SlideCheckboxControl', EXTENDS(chlk.controls.CheckboxControl), [
            OVERRIDE, VOID, function onCreate_() {
                BASE();
                ASSET('~/assets/jade/controls/slide-checkbox.jade')(this);
            }
        ]);
});