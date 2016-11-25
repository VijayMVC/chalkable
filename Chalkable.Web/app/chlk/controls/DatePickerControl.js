REQUIRE('chlk.controls.Base');
REQUIRE('chlk.models.common.ChlkDate');

NAMESPACE('chlk.controls', function () {

    chlk.controls.updateDatePicker = function (scope, node, value) {
        var value = new chlk.models.common.ChlkSchoolYearDate.$createServerTime(node.getData('value'));
        chlk.controls.DatePickerControl.prototype.updateDatePicker.call(scope, node, value, true);
    };

    var wasChange, underOverlayCls = 'under-overlay';

    /** @class chlk.controls.DatePickerControl */
    CLASS(
        'DatePickerControl', EXTENDS(chlk.controls.Base), [
            OVERRIDE, VOID, function onCreate_() {
                BASE();
                ASSET('~/assets/jade/controls/date-picker.jade')(this);
            },

            Date, 'value',

            OVERRIDE, Date, function getServerDate(str_,a_,b_){
                return this.getSchoolYearServerDate(str_,a_,b_);
            },

            [[String, Object, Object]],
            Object, function processAttrs(name, value_, attrs_) {
                attrs_.id = attrs_.id || ria.dom.Dom.GID();
                var options = attrs_['data-options'];
                attrs_.name = options.dateFormat ? name + 'formatted' : name;

                if (value_){
                   if(!value_.format)
                        value_ = chlk.models.common.ChlkDate(getDate(value_));
                    attrs_.value = value_.format(options.dateFormat || 'mm/dd/yy');
                }

                var options = attrs_['data-options'];
                var controller = attrs_['data-controller'];
                if(controller){
                    var action = attrs_['data-action'];
                    var params = attrs_['data-params'] || [];
                    var that = this;
                    options.onSelect = function (dateText, inst) {
                        var date = new chlk.models.common.ChlkSchoolYearDate.$createServerTime(new Date(dateText));
                        params.push(date);
                        var state = that.context.getState();
                        state.setController(controller);
                        state.setAction(action);
                        state.setParams(params);
                        state.setPublic(false);
                        that.context.stateUpdated();
                    }
                }

                if(options.inCurrentGradingPeriods) {
                    var gradingPeriods = this.getContext().getSession().get(ChlkSessionConstants.GRADING_PERIODS);
                    var ranges = gradingPeriods
                        .map(function (_) {
                            return {
                                start: _.getStartDate().getDate(),
                                end: _.getEndDate().getDate()
                            }
                        });

                    ranges.sort(function (_1, _2) { return _1.start - _2.start; });
                    /*options.beforeShowDay = function(date) {
                        return [ranges.some(function (_) { return _.start <= date && date <= _.end }), ''];
                    };*/
                    options.minDate = ranges[0].start;
                    options.maxDate = ranges[ranges.length - 1].end;
                }

                if(options.enabledDays) {
                    var enabledDays = options.enabledDays.map(function(day){return day.format('mm-dd-yy')});
                    delete options.enabledDays;
                    options.beforeShowDay = function(date) {
                        return [enabledDays.indexOf(date.format('m-d-Y')) > -1, ''];
                    };
                }

                if(options.dateRanges) {
                    var ranges = options.dateRanges.sort(function (_1, _2) { return _1.start - _2.start; });
                    delete options.dateRanges;
                    options.beforeShowDay = function(date) {
                        return [ranges.some(function (_) { return _.start <= date && date <= _.end }), ''];
                    };
                    options.minDate = ranges[0].start;
                    options.maxDate = ranges[ranges.length - 1].end;
                }

                if(options.inCurrentMp && !this.userIsAdmin() ){
                    var gp = this.getContext().getSession().get(ChlkSessionConstants.GRADING_PERIOD);
                    if(!options.minDate || options.minDate < gp.getStartDate().getDate())
                        options.minDate = gp.getStartDate().getDate();
                    if(!options.maxDate || options.maxDate > gp.getEndDate().getDate())
                        options.maxDate = gp.getEndDate().getDate();
                }

                if(options.inCurrentSchoolYear ){
                    var sy = this.getContext().getSession().get(ChlkSessionConstants.SCHOOL_YEAR);
                    if(!options.minDate || options.minDate < sy.getStartDate().getDate())
                        options.minDate = sy.getStartDate().getDate();
                    if(!options.maxDate || options.maxDate > sy.getEndDate().getDate())
                        options.maxDate = sy.getEndDate().getDate();
                }

                if(options.calendarCls || options.underOverlay){
                    options.beforeShow = function(){
                        if(options.calendarCls)
                            jQuery('#ui-datepicker-div').addClass(options.calendarCls);

                        if(options.underOverlay)
                            jQuery('#ui-datepicker-div').addClass(underOverlayCls);
                    }
                }

                if(options.endRangeSelector){
                    options.onClose = function( selectedDate ) {
                        var node = $(this);

                        setTimeout(function(){
                            if(node.data('wasChange')){
                                if(options.dateFormat){
                                    var dt = Date.parse(selectedDate) || $(options.endRangeSelector).data('options').minDate;
                                    selectedDate = $.datepicker.formatDate( options.dateFormat, dt );
                                }

                                $(options.endRangeSelector).datepicker( "option", "minDate", selectedDate );
                                $(options.endRangeSelector).trigger('blur');
                            }

                            node.data('wasChange', false);
                        }, 1);

                    }
                }

                if(options.startRangeSelector){
                    options.onClose = function( selectedDate ) {
                        var node = $(this);

                        setTimeout(function(){
                            if(node.data('wasChange')){
                                if(options.dateFormat){
                                    var dt = Date.parse(selectedDate) || $(options.startRangeSelector).data('options').maxDate;
                                    selectedDate = $.datepicker.formatDate( options.dateFormat, dt );
                                }

                                $(options.startRangeSelector).datepicker( "option", "maxDate", selectedDate );
                                $(options.startRangeSelector).trigger('blur');
                            }

                            node.data('wasChange', false);
                        }.bind(this), 1);


                    }
                }

                this.queueReanimation_(attrs_.id, options, value_);

                return attrs_;
            },

            function updateValue(node, needChange_){
                var options = node.getData('options'),
                    value = node.getValue();
                if(options.dateFormat && value){
                    var dt = Date.parse(node.getValue());
                    var newValue = $.datepicker.formatDate( options.dateFormat, dt );
                    if(newValue != value){
                        node.setValue(newValue);
                        node.trigger('change');
                    }

                }
            },

            function updateDatePicker(node, value_, options_, noClearValue_){
                var options = options_ || node.getData('options'), that = this;
                this.reanimate_(node, options, value_);
                if(!value_ && !noClearValue_)
                    node.setValue('');
                node.off('change.datepiker');
                node.on('change.datepiker', function(node, event){
                    node.setData('wasChange', true);
                    var options = node.getData('options');
                    var value = jQuery(node.valueOf()).datepicker('getDate');
                    if(value){
                        if(options.minDate){
                            var min = that.getServerDate(options.minDate);
                            if(value < min){
                                value = min;
                                node.$.datepicker('setDate', min);
                            }
                        }
                        if(options.maxDate){
                            var max = that.getServerDate(options.maxDate);
                            if(value > max){
                                value = max;
                                node.$.datepicker('setDate', max);
                            }
                        }
                        node.next().setValue(value.format('m/d/Y'));
                        node.setData('value', value.format('m/d/Y'));
                    }else
                        node.next().setValue('');
                });

                node.off('blur.datepiker');
                node.on('blur.datepiker', function(node, event){
                    that.updateValue(node);
                });
            },

            VOID, function queueReanimation_(id, options, value_) {
                this.context.getDefaultView()
                    .onActivityRefreshed(function (activity, model) {
                        var that = this;
                        var node = ria.dom.Dom('#' + id);
                        node.setData('options', options);

                        node.off('keydown.datepiker');
                        node.on('keydown.datepiker', function(node, event){
                            if (event.which == ria.dom.Keys.ENTER.valueOf()) {
                                var input = node.getValue();
                                var date = Date.parseExact(input, 'M/d/yy');
                                if(date) {
                                    node.setValue(date.toString("MMM d, yyyy"));
                                    node.datepicker(options, date);
                                }

                                that.updateValue(node);

                                //if(date)
                                    node.trigger('change');
                            }
                        });

                        this.updateDatePicker(node, value_, options);
                    }.bind(this));
            },

            [ria.mvc.DomEventBind('click', '.calendar-icon')],
            [[ria.dom.Dom, ria.dom.Event]],
            function starClick(node, event){
                node.parent().find('.hasDatepicker').trigger('focus');
            },

            [[ria.dom.Dom, Object, Object]],
            VOID, function reanimate_(node, options, value_) {
                var defaultOptions = {dateFormat: "mm/dd/yy"};
                node.datepicker(ria.__API.merge(options,defaultOptions), value_ && value_.getDate());
                node.setData('control', this);
            }
        ]);
});