REQUIRE('chlk.controls.Base');
REQUIRE('chlk.models.common.ChlkDate');

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
                    var mp = this.getContext().getSession().get('markingPeriod');
                    options.minDate = mp.getStartDate().getDate();
                    options.maxDate = mp.getEndDate().getDate();
                }

                if(options.calendarCls){
                    options.beforeShow = function(){
                        jQuery('#ui-datepicker-div').addClass(options.calendarCls);
                    }
                }

                this.queueReanimation_(attrs.id, options, value);

                return attrs;
            },

            VOID, function queueReanimation_(id, options, value) {
                this.context.getDefaultView()
                    .onActivityRefreshed(function (activity, model) {
                        var node = ria.dom.Dom('#' + id);
                        this.reanimate_(node, options, value, activity, model)
                        if(!value)
                            node.setValue('');
                    }.bind(this));
            },

            [[ria.dom.Dom, Object, Object, ria.mvc.IActivity, Object]],
            VOID, function reanimate_(node, options, value, activity, model) {
                var defaultOptions = {dateFormat: "mm/dd/yy"};
                node.datepicker(ria.__API.extendWithDefault(options,defaultOptions), value.getDate());
            }
        ]);
});