REQUIRE('chlk.activities.lib.TemplatePage');

NAMESPACE('chlk.activities.feed', function () {

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

            [ria.mvc.DomEventBind('change keypress', '.start-end-picker')],
            [[ria.dom.Dom, ria.dom.Event, Object]],
            VOID, function dateSelect(node, event, selected_){
                if(event.type == 'change' || event.which == ria.dom.Keys.ENTER.valueOf()){
                    var btn = this.dom.find('#date-ok-button');
                    if(!this.dom.find('#toDate').getValue() || !this.dom.find('#toDate').getValue())
                        btn.setAttr('disabled', true);
                    else
                        btn.removeAttr('disabled');
                }
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

                this.updateCopySubmitBtn_();
            },

            [ria.mvc.DomEventBind('change', '.feed-item-check')],
            [[ria.dom.Dom, ria.dom.Event, Object]],
            VOID, function feedItemSelect(node, event, selected_){
                this.updateCopySubmitBtn_();
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
                this.dom.find('.copy-activities.active').trigger('click');
                this.dom.find('.tools-buttons-block').toggleClass('hidden');
                node.toggleClass('active');
            },

            [ria.mvc.DomEventBind('click', '.copy-activities')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function copyActivitiesClick(node, event){
                this.dom.find('.feed-tools.active').trigger('click');
                this.dom.find('.feed-container').toggleClass('copy-mode');
                node.toggleClass('active');
            },

            [ria.mvc.DomEventBind('click', '.cancel-copy')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function cancelCopyClick(node, event){
                this.dom.find('.copy-activities').trigger('click');
            },

            [ria.mvc.DomEventBind('click', '.copy-submit')],
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
                this.dom.find('.announcements-to-copy').setValue(value);
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
                this.dom.find('.all-tasks-check')
                    .trigger(chlk.controls.CheckBoxEvents.CHANGE_VALUE.valueOf(), [false]);

                this.dom.find('.feed-item-check ').trigger('change');

                this.dom.find('.selected-item').removeClass('selected-item');
            },

            [ria.mvc.DomEventBind('change', '.copy-to-select')],
            [[ria.dom.Dom, ria.dom.Event, Object]],
            VOID, function classSelect(node, event, selected_){
                this.updateCopySubmitBtn_();
            },

            function updateCopySubmitBtn_(){
                var btn = this.dom.find('.copy-submit');
                if(this.dom.find('.feed-item-check:checked').count() > 0 && this.dom.find('[name="toClassId"]').getValue()){
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

            /*[ria.mvc.DomEventBind('mousedown', '#sort-select .chosen-drop')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function chosenDropClick(node, event){
                this._dropDownClicked = true;
                var input = this.dom.find('#sort-select-input');
                setTimeout(function(){
                    input.trigger('focus');
                }, 1);

            },

            [ria.mvc.DomEventBind('blur', '#sort-select-input')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function sortBlur(node, event){
                if(!this._dropDownClicked){
                    var select = this.dom.find('#sort-select');
                    select.removeClass('chosen-container-active');
                    select.removeClass('chosen-with-drop');
                }

                this._dropDownClicked = false;
            },*/

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
                if(model instanceof chlk.models.feed.FeedItems){
                    this.dom.find('.feed-grid').trigger(chlk.controls.GridEvents.UPDATED.valueOf());
                    if(!model.getItems().length)
                        this.dom.find('.form-for-grid').trigger(chlk.controls.FormEvents.DISABLE_SCROLLING.valueOf());
                }
            },

            OVERRIDE, VOID, function onRefresh_(model) {
                BASE(model);
                this.dom.find('select.prepared').removeClass('prepared');
            },

            OVERRIDE, VOID, function onStop_() {
                BASE();
                new ria.dom.Dom().off('click.dates');
            }

        ]);
});