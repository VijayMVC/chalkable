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

            [ria.mvc.DomEventBind('change', '.start-end-picker')],
            [[ria.dom.Dom, ria.dom.Event, Object]],
            VOID, function dateSelect(node, event, selected_){
                var btn = this.dom.find('#date-ok-button');
                if(!this.dom.find('#toDate').getValue() || !this.dom.find('#toDate').getValue())
                    btn.setAttr('disabled', true);
                else
                    btn.removeAttr('disabled');
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
                this.dom.find('.left-buttons-block').toggleClass('hidden');
                node.toggleClass('active');
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