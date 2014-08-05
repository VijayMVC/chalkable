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
            OVERRIDE, VOID, function update(node){
                BASE(node);
                jQuery(node).carouFredSel({
                    width: "variable",
                    align: "center",
                    circular: false,
                    auto: false,
                    prev: " #api_list_prev",
                    next: "#api_list_next"
                });
            },

            [[Object]],
            OVERRIDE, Object, function processAttrs(attributes) {
                return BASE(attributes);
                //todo: add options
            }

        ]);
});