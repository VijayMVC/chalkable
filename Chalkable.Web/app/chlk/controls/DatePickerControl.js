REQUIRE('chlk.controls.Base');

NAMESPACE('chlk.controls', function () {

    /** @class chlk.controls.DatePickerControl */
    CLASS(
        'DatePickerControl', EXTENDS(ria.mvc.DomControl), [
            OVERRIDE, VOID, function onCreate_() {
                BASE();
                ASSET('~/assets/jade/controls/date-picker.jade')(this);
            },

            Date, 'value',

            [[String, Object, Object]],
            Object, function processAttrs(name, value, attrs) {
                attrs.id = attrs.id || ria.dom.NewGID();
                attrs.name = name;
                if (typeof value !== 'undefined')
                    attrs.value = value.toString('/');

                var options = attrs['data-options'];
                this.queueReanimation_(attrs.id, options);
                this.setValue(value.getDate());

                return attrs;
            },

            VOID, function queueReanimation_(id, options) {
                this.context.getDefaultView()
                    .onActivityRefreshed(function (activity, model) {
                        this.reanimate_(ria.dom.Dom('#' + id), options, activity, model)
                    }.bind(this));
            },

            [[ria.dom.Dom, Object, ria.mvc.IActivity, Object]],
            VOID, function reanimate_(node, options, activity, model) {
                var defaultOptions = {dateFormat: "mm/dd/yy"};
                node.datepicker(ria.__API.extendWithDefault(options,defaultOptions), this.getValue());
            }
        ]);
});