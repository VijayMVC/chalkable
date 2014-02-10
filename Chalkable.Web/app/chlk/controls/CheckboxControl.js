REQUIRE('chlk.controls.Base');

NAMESPACE('chlk.controls', function () {

    /** @class chlk.controls.CheckboxControl */
    CLASS(
        'CheckboxControl', EXTENDS(chlk.controls.Base), [
            OVERRIDE, VOID, function onCreate_() {
                BASE();
                ASSET('~/assets/jade/controls/checkbox.jade')(this);
            },

            [[String, Boolean]],
            VOID, function prepareData(name, value) {
                this.context.getDefaultView()
                    .onActivityRefreshed(function (activity, model) {
                        var hidden = activity.getDom().find('.hidden-checkbox[name=' + name + ']');
                        activity.getDom().find('#' + name).on('change', function(){
                            var lastValue = hidden.getData('value');
                            var newValue = !lastValue;
                            hidden.setData('value', newValue);
                            hidden.setValue(newValue);
                        });
                        hidden.setData('value', value || false);
                    }.bind(this));
            }
        ]);
});