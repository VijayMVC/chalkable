REQUIRE('chlk.controls.Base');

NAMESPACE('chlk.controls', function () {

    /** @class chlk.controls.MaskedInputControl */
    CLASS(
        'MaskedInputControl', EXTENDS(chlk.controls.Base), [
            OVERRIDE, VOID, function onCreate_() {
                BASE();
                ASSET('~/assets/jade/controls/masked-input.jade')(this);
            },

            [[String, String]],
            VOID, function prepareData(name, mask) {
                this.context.getDefaultView()
                    .onActivityRefreshed(function (activity, model) {
                    var input = activity.getDom().find('input[name=' + name + ']');
                    if(mask){
                        jQuery(input.valueOf()).mask(mask);
                    }
                }.bind(this));
            }
        ]);
});