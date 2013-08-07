REQUIRE('chlk.controls.Base');

NAMESPACE('chlk.controls', function () {

    /** @class chlk.controls.SlideCheckboxControl*/
    CLASS(
        'SlideCheckboxControl', EXTENDS(chlk.controls.Base), [
            OVERRIDE, VOID, function onCreate_() {
                BASE();
                ASSET('~/assets/jade/controls/slide-checkbox.jade')(this);
            },


            [ria.mvc.DomEventBind('click', '.slide-checkbox')],
            [[ria.dom.Dom, ria.dom.Event]],
            Boolean, function changed_($target, event) {
                var checkbox = $target.find('input[type=checkbox]');
                if (checkbox){
                    checkbox.setAttr('checked', !checkbox.is(':checked'));
                }

            }
        ]);
});