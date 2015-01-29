REQUIRE('chlk.controls.Base');

NAMESPACE('chlk.controls', function () {

    /** @class chlk.controls.ButtonControl */
    CLASS(
        'ButtonControl', EXTENDS(chlk.controls.Base), [
            OVERRIDE, VOID, function onCreate_() {
                BASE();
                ASSET('~/assets/jade/controls/button.jade')(this);
            },

            [[Object]],
            Object, function processAttrs(attributes){
                attributes['class'] = attributes['class'] || [];
                if (attributes.disabled)
                    attributes['class'].push('disabled');

                return attributes;
            }
        ]);
});