REQUIRE('chlk.activities.lib.TemplatePage');

NAMESPACE('chlk.activities.feed', function () {

    var minActivityDate, formatedScheduledDays, scheduledDays;

    /** @class chlk.activities.feed.BaseFeedPage*/
    CLASS(
        'BaseFeedPage', EXTENDS(chlk.activities.lib.TemplatePage), [

            function prepareSelect(node){
                var value = parseInt(node.getValue());
                if(!value || value < 1){
                    node.addClass('prepared');
                    node.setValue('');
                    setTimeout(function(){
                        node.removeClass('prepared');
                    }, 10)
                }

            },

            [ria.mvc.DomEventBind('submit', 'form')],
            [[ria.dom.Dom, ria.dom.Event]],
            function formSubmit(node, event){
                var fromNode = node.find('[name="fromdate"]'),
                    toNode = node.find('[name="todate"]');
                if(!fromNode.getValue() && toNode.getValue() || fromNode.getValue() && !toNode.getValue()){
                    node.find('.date-range').find('input').setValue('');
                }

                this.prepareSelect(this.dom.find('.gradingPeriodSelect'));
                this.prepareSelect(this.dom.find('.annTypeSelect'));
            },

            [ria.mvc.DomEventBind('change', '.markDoneSelect')],
            [[ria.dom.Dom, ria.dom.Event, Object]],
            VOID, function markDoneSelect(node, event, selected_){
                this.dom.find('#mark-done-submit').trigger('click');
            },

            [ria.mvc.DomEventBind('change', 'select')],
            [[ria.dom.Dom, ria.dom.Event, Object]],
            VOID, function changeSelect(node, event, selected_){
                if(!node.hasClass('prepared'))
                    this.dom.find('.to-set').setValue('true');
            },

            [ria.mvc.DomEventBind('change keydown', '.start-end-picker')],
            [[ria.dom.Dom, ria.dom.Event, Object]],
            VOID, function dateSelect(node, event, selected_){console.info(event.which);
                //if(event.type == 'change' || event.which == ria.dom.Keys.ENTER.valueOf()){
                    var btn = this.dom.find('#date-ok-button');
                    if(!this.dom.find('#toDate').getValue() || !this.dom.find('#toDate').getValue())
                        btn.setAttr('disabled', true);
                    else
                        btn.removeAttr('disabled');
                //}
            },

            [ria.mvc.DomEventBind('change', '.gradingPeriodSelect')],
            [[ria.dom.Dom, ria.dom.Event, Object]],
            VOID, function gradingPeriodSelect(node, event, selected_){
                if(!node.hasClass('prepared'))
                    setTimeout(function(){
                        if(node.getValue() == -1)
                            this.dom.find('.date-range-popup').removeClass('hidden');
                        else{
                            this.dom.find('.start-end-picker').next().setValue('');
                            this.dom.find('#sort-submit').trigger('click');
                        }

                    }.bind(this), 10);
            },

            [ria.mvc.DomEventBind('change', '.submit-after-select')],
            [[ria.dom.Dom, ria.dom.Event, Object]],
            VOID, function submitAfterSelect(node, event, selected_){
                if(!node.hasClass('prepared'))
                    setTimeout(function(){
                        this.dom.find('#sort-submit').trigger('click');
                    }.bind(this), 10);
            },

            [ria.mvc.DomEventBind('change', '.all-tasks-check')],
            [[ria.dom.Dom, ria.dom.Event, Object]],
            VOID, function allTasksSelect(node, event, selected_){
                this.dom.find('.feed-item-check').forEach(function(element){
                    element.trigger(chlk.controls.CheckBoxEvents.CHANGE_VALUE.valueOf(), [node.is(':checked')]);
                });

                if(this.dom.find('.feed-container').hasClass('adjust-mode'))
                    this.updateMinActivityDate_();

                this.updateTopSubmitBtn_();
            },

            [ria.mvc.DomEventBind('change', '.feed-item-check')],
            [[ria.dom.Dom, ria.dom.Event, Object]],
            VOID, function feedItemSelect(node, event, selected_){
                if(this.dom.find('.feed-container').hasClass('adjust-mode'))
                    this.updateMinActivityDate_();

                this.updateTopSubmitBtn_();

                var allCheck = this.dom.find('.all-tasks-check:visible');
                if(!node.is(':checked') && allCheck.is(':checked'))
                    allCheck.trigger(chlk.controls.CheckBoxEvents.CHANGE_VALUE.valueOf(), [false]);
            },

            [ria.mvc.DomEventBind('click', '.gradingPeriodSelect + DIV li:last-child')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function dateRangeClick(node, event){
                setTimeout(function(){
                    this.dom.find('.date-range-popup').removeClass('hidden')
                }.bind(this), 10);
            },

            [ria.mvc.DomEventBind('click', '.latest-earliest-option')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function sortOptionClick(node, event){
                this.dom.find('.latest-earliest-input').setValue(!!node.getData('value'));
                this.sortSubmit();
            },

            [ria.mvc.DomEventBind('click', '.feed-tools')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function feedToolsClick(node, event){
                this.dom.find('.top-block-btn:not(.feed-tools).active').trigger('click');
                this.dom.find('.feed-container').toggleClass('settings-mode');
                node.toggleClass('active');
            },

            [ria.mvc.DomEventBind('click', '.copy-activities')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function copyActivitiesClick(node, event){
                this.dom.find('.top-block-btn:not(.copy-activities).active').trigger('click');
                this.dom.find('.feed-container').toggleClass('copy-mode');
                node.toggleClass('active');
            },

            [ria.mvc.DomEventBind('click', '.adjust-activities')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function adjustActivitiesClick(node, event){
                this.dom.find('.top-block-btn:not(.adjust-activities).active').trigger('click');
                this.dom.find('.feed-container').toggleClass('adjust-mode');
                node.toggleClass('active');
            },

            [ria.mvc.DomEventBind('click', '.cancel-copy, .cancel-adjust')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function topCancelClick(node, event){
                this.dom.find('.feed-item-check').forEach(function(element){
                    element.trigger(chlk.controls.CheckBoxEvents.CHANGE_VALUE.valueOf(), [false]);
                });

                this.updateTopSubmitBtn_();
            },

            [ria.mvc.DomEventBind('click', '.cancel-copy')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function cancelCopyClick(node, event){
                this.dom.find('.copy-activities').trigger('click');
            },

            [ria.mvc.DomEventBind('click', '.cancel-adjust')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function cancelAdjustClick(node, event){
                this.dom.find('.adjust-activities').trigger('click');
                setTimeout(function(){
                    this.updateMinActivityDate_();
                }.bind(this), 1);
            },

            [ria.mvc.DomEventBind('click', '.submit-selected')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function copySubmit(node, event){
                var announcements = [];
                this.dom.find('.feed-item-check:checked').forEach(function(node){
                    announcements.push({
                        announcementId: node.getData('id'),
                        announcementType: node.getData('type')
                    });
                });

                var value = announcements.length ? JSON.stringify(announcements) : '';
                this.dom.find('.selected-announcements').setValue(value);
            },

            [ria.mvc.PartialUpdateRule(null, 'announcements-copy')],
            VOID, function copyUpdate(tpl, model, msg_) {
                this.dom.find('.copy-activities').trigger('click');
                var select = this.dom.find('.copy-to-select');
                select.find('.selected-value').setValue('');
                select.find('.value-text').setText('');
                this.dom.find('.copy-start-date')
                    .setValue('')
                    .setData('value', null)
                    .trigger('change');
                this.dom.find('.all-tasks-check:visible')
                    .trigger(chlk.controls.CheckBoxEvents.CHANGE_VALUE.valueOf(), [false]);

                this.dom.find('.feed-item-check ').trigger('change');

                this.dom.find('.selected-item').removeClass('selected-item');
            },

            [ria.mvc.DomEventBind('change', '.copy-to-select')],
            [[ria.dom.Dom, ria.dom.Event, Object]],
            VOID, function classSelect(node, event, selected_){
                this.updateTopSubmitBtn_();
            },

            function updateTopSubmitBtn_(){
                var btn = this.dom.find('.top-submit-btn:visible');
                if(this.dom.find('.feed-item-check:checked').count() > 0 &&
                    (this.dom.find('.feed-container').hasClass('adjust-mode') && this.dom.find('.adjust-start-date').getValue() ||
                    this.dom.find('.feed-container').hasClass('copy-mode') && this.dom.find('[name="toClassId"]').getValue())){
                        btn.removeAttr('disabled');
                        btn.setProp('disabled', false);
                }else{
                    btn.setAttr('disabled', 'disabled');
                    btn.setProp('disabled', true);
                }
            },

            [ria.mvc.DomEventBind('click', '.clear-filters')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function clearFiltersClick(node, event){
                this.dom.find('select:not(.markDoneSelect), .start-end-picker').setValue('');
                this.dom.find('.to-set').setValue('true');
                this.dom.find('#sort-submit').trigger('click');
            },

            function sortSubmit(){
                this.dom.find('#sort-submit').trigger('click');
                var select = this.dom.find('#sort-select');
                select.removeClass('chosen-container-active');
                select.removeClass('chosen-with-drop');
            },

            function $() {
                BASE();
                //this._dropDownClicked = false;
            },

            OVERRIDE, VOID, function onRender_(model) {
                BASE(model);
                var that = this;
                new ria.dom.Dom().on('click.dates', function (node, event) {
                    var target = new ria.dom.Dom(event.target), dom = that.dom, popup = dom.find('.date-range-popup');
                    if (!target.is('#sort-submit') && !popup.hasClass('hidden') && !target.isOrInside('.date-range-popup') && !target.isOrInside('.ui-datepicker-header') && !target.isOrInside('.ui-datepicker-calendar')) {
                        popup.addClass('hidden');
                        dom.find('.to-set').setValue('');
                    }
                });
            },

            OVERRIDE, VOID, function onPartialRefresh_(model, msg_) {
                BASE(model, msg_);
                this.dom.find('select.prepared').removeClass('prepared');

                if(model instanceof chlk.models.feed.Feed){
                    minActivityDate = null;
                    formatedScheduledDays = model.getClassScheduledDays() && model.getClassScheduledDays().map(function(item){return item.format('mm-dd-yy')});
                    scheduledDays = model.getClassScheduledDays() && model.getClassScheduledDays().map(function(item){return item.getDate()});
                }

                if(model instanceof chlk.models.feed.FeedItems){
                    this.dom.find('.feed-grid').trigger(chlk.controls.GridEvents.UPDATED.valueOf());
                    if(!model.getItems().length)
                        this.dom.find('.form-for-grid').trigger(chlk.controls.FormEvents.DISABLE_SCROLLING.valueOf());
                }
            },

            OVERRIDE, VOID, function onRefresh_(model) {
                BASE(model);
                minActivityDate = null;
                formatedScheduledDays = model.getClassScheduledDays() && model.getClassScheduledDays().map(function(item){return item.format('mm-dd-yy')});
                scheduledDays = model.getClassScheduledDays() && model.getClassScheduledDays().map(function(item){return item.getDate()});
                this.dom.find('select.prepared').removeClass('prepared');
            },

            OVERRIDE, VOID, function onStop_() {
                BASE();
                new ria.dom.Dom().off('click.dates');
            },

            // ---------------------- ADJUST DAYS -------------------

            [ria.mvc.DomEventBind('change', '.adjust-days-count')],
            [[ria.dom.Dom, ria.dom.Event, Object]],
            VOID, function adjustDaysCountChange(node, event, selected_){
                if(minActivityDate && formatedScheduledDays){
                    var curPosition = formatedScheduledDays.indexOf(minActivityDate.format('m-d-Y')),
                        value = parseInt(node.getValue(), 10),
                        selectedPosition = curPosition + value,
                        date;

                    if(selectedPosition < 0){
                        selectedPosition = 0;
                        node.setValue(-curPosition);
                    }else{
                        if(selectedPosition >= formatedScheduledDays.length){
                            selectedPosition = formatedScheduledDays.length - 1;
                            node.setValue(formatedScheduledDays.length - 1 - curPosition)
                        }
                    }

                    date = scheduledDays[selectedPosition];
                    this.dom.find('.adjust-start-date').$.datepicker('setDate', date);
                }
            },

            [ria.mvc.DomEventBind('change', '.adjust-start-date')],
            [[ria.dom.Dom, ria.dom.Event, Object]],
            VOID, function adjustStartDateChange(node, event, selected_){
                var curMinPos = formatedScheduledDays.indexOf(minActivityDate.format('m-d-Y')),
                    selectedDate = node.$.datepicker('getDate');

                if(selectedDate){
                    var selectedPos = formatedScheduledDays.indexOf(selectedDate.format('m-d-Y'));

                    if(selectedPos == -1){
                        var lastDay = scheduledDays[scheduledDays.length - 1],
                            firstDay = scheduledDays[0], curDate;

                        if(selectedDate > lastDay){
                            curDate = lastDay;
                            selectedPos = scheduledDays.length - 1;
                        }else{
                            if(selectedDate < firstDay){
                                curDate = firstDay;
                                selectedPos = 0;
                            }else{
                                var diff = selectedDate - firstDay, curItem = scheduledDays[0], curDiff;
                                scheduledDays.forEach(function(item, i){
                                    curDiff = Math.abs(selectedDate - item);
                                    if(curDiff < diff){
                                        diff = curDiff;
                                        curItem = item;
                                        selectedPos = i;
                                    }
                                });

                                curDate = curItem;
                            }
                        }

                        node.$.datepicker('setDate', curDate);
                    }

                    this.dom.find('.adjust-days-count').setValue(selectedPos - curMinPos);
                }else{
                    this.updateTopSubmitBtn_();
                }
            },

            [ria.mvc.DomEventBind('keydown', '.adjust-start-date, .adjust-days-count')],
            [[ria.dom.Dom, ria.dom.Event, Object]],
            function adjustKeyPress(node, event, selected_) {
                if (event.which == ria.dom.Keys.ENTER.valueOf()) {
                    event.preventDefault();
                    node.trigger('change');
                    return false;
                }
            },

            function updateMinActivityDate_() {
                var checks = this.dom.find('.feed-item-check:checked').$;
                minActivityDate = undefined;

                if(checks.length){
                    var curMinDate = scheduledDays[scheduledDays.length - 1], minSelected,
                        dates = checks.map(function(){return getDate($(this).data('date'))});

                    dates = dates.sort(function(a,b){return a - b});
                    minSelected = dates[0];

                    if(minSelected < curMinDate){
                        if(formatedScheduledDays.indexOf(minSelected.format('m-d-Y')) > -1){
                            curMinDate = minSelected;
                        }else{
                            var firstDay = scheduledDays[0];

                            if(minSelected < firstDay){
                                curMinDate = firstDay;
                            }else{
                                var diff = minSelected - firstDay, curItem = scheduledDays[0], curDiff;
                                scheduledDays.forEach(function(item, i){
                                    curDiff = Math.abs(minSelected - item);
                                    if(curDiff < diff){
                                        diff = curDiff;
                                        curItem = item;
                                    }
                                });

                                curMinDate = curItem;
                            }
                        }
                    }

                    minActivityDate = curMinDate;
                }

                this.dom.find('.adjust-start-date').$.datepicker('setDate', minActivityDate);
                this.dom.find('.adjust-days-count').setValue('0');

                var nodes = this.dom.find('.adjust-start-date, .adjust-days-count');
                if(minActivityDate){
                    nodes.removeAttr('disabled');
                    nodes.setProp('disabled', false);
                }else{
                    nodes.setAttr('disabled', 'disabled');
                    nodes.setProp('disabled', true);
                }

            }
        ]);
});