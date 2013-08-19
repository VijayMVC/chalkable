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
                if(attributes.disabled)
                    if(Array.isArray(attributes.class)) {
                        attributes.class = attributes.class || [];
                        attributes.class.push('disabled');
                    }else{
                        attributes.class = attributes.class + ' disabled';
                    }

                return attributes;
            }
        ]);
});