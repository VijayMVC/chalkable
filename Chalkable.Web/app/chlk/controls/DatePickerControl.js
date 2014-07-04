REQUIRE('chlk.controls.Base');
REQUIRE('chlk.models.common.ChlkDate');

NAMESPACE('chlk.controls', function () {

    chlk.controls.updateDatePicker = function (scope, node, value) {
        var value = new chlk.models.common.ChlkDate(getDate(node.getValue()));
        chlk.controls.DatePickerControl.prototype.updateDatePicker.call(scope, node, value, true);
    };

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
                attrs.id = attrs.id || ria.dom.Dom.GID();
                attrs.name = name;
                if (value)
                    attrs.value = value.format('mm/dd/yy');

                var options = attrs['data-options'];
                var controller = attrs['data-controller'];
                if(controller){
                    var action = attrs['data-action'];
                    var params = attrs['data-params'] || [];
                    var that = this;
                    options.onSelect = function (dateText, inst) {
                        var date = new chlk.models.common.ChlkDate(getDate(dateText));
                        params.push(date);
                        var state = that.context.getState();
                        state.setController(controller);
                        state.setAction(action);
                        state.setParams(params);
                        state.setPublic(false);
                        that.context.stateUpdated();
                    }
                }

                if(options.inCurrentMp){
                    var gp = this.getContext().getSession().get(ChlkSessionConstants.GRADING_PERIOD);
                    options.minDate = gp.getStartDate().getDate();
                    options.maxDate = gp.getEndDate().getDate();
                }

                if(options.calendarCls){
                    options.beforeShow = function(){
                        jQuery('#ui-datepicker-div').addClass(options.calendarCls);
                    }
                }

                this.queueReanimationCustom_(attrs.id, options, value);

                return attrs;
            },

            function updateDatePicker(node, value, options_, noClearValue_){
                var options = options_ || node.getData('options');
                this.reanimateCustom_(node, options, value);
                if(!value && !noClearValue_)
                    node.setValue('');
                node.off('change.datepiker');
                node.on('change.datepiker', function(node, event){
                    var options = node.getData('options');
                    if(options.minDate){
                        var min = getDate(options.minDate);
                        if(getDate(node.getValue()) < min)
                            node.setValue(new chlk.models.common.ChlkDate(min).format('mm/dd/yy'));
                    }
                    if(options.maxDate){
                        var max = getDate(options.maxDate);
                        if(getDate(node.getValue()) > max)
                            node.setValue(new chlk.models.common.ChlkDate(max).format('mm/dd/yy'));
                    }
                })
            },

            VOID, function queueReanimationCustom_(id, options, value) {
                this.context.getDefaultView()
                    .onActivityRefreshed(function (activity, model) {
                        var node = ria.dom.Dom('#' + id);
                        node.setData('options', options);
                        this.updateDatePicker(node, value, options);
                    }.bind(this));
            },

            [[ria.dom.Dom, Object, Object]],
            VOID, function reanimateCustom_(node, options, value) {
                var defaultOptions = {dateFormat: "mm/dd/yy"};
                node.datepicker(ria.__API.extendWithDefault(options,defaultOptions), value && value.getDate());
                node.setData('control', this);
            }
        ]);
});