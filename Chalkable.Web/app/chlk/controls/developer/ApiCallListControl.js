REQUIRE('chlk.controls.BaseJQueryControl');

NAMESPACE('chlk.controls.developer', function () {

    var hideClass = 'x-hidden';

    /** @class chlk.controls.developer.ApiCallListControl */
    CLASS(
        'ApiCallListControl', EXTENDS(chlk.controls.BaseJQueryControl), [
            OVERRIDE, VOID, function onCreate_() {
                BASE();
                ASSET('~/assets/jade/controls/developer/api-call-list.jade')(this);
            },

            [[Object]],
            OVERRIDE, Object, function processAttrs(attributes) {
                return BASE(attributes);
                //todo: add options
            }

        ]);
});
