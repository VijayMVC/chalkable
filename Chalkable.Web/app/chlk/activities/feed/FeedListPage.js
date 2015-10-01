REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.feed.Feed');
REQUIRE('chlk.templates.feed.NotificationsCount');
REQUIRE('chlk.templates.announcement.FeedItemTpl');
REQUIRE('chlk.templates.announcement.FeedItemsTpl');

NAMESPACE('chlk.activities.feed', function () {

    var interval;

    /** @class chlk.activities.feed.FeedListPage*/
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.feed.Feed)],
        [ria.mvc.PartialUpdateRule(chlk.templates.feed.Feed, null, null, ria.mvc.PartialUpdateRuleActions.Replace)],
        [ria.mvc.PartialUpdateRule(chlk.templates.feed.NotificationsCount, 'notifications', '.feed-notifications', ria.mvc.PartialUpdateRuleActions.Replace)],
        [ria.mvc.PartialUpdateRule(chlk.templates.announcement.FeedItemsTpl, null, '.chlk-grid', ria.mvc.PartialUpdateRuleActions.Append)],
        'FeedListPage', EXTENDS(chlk.activities.lib.TemplatePage), [

            [ria.mvc.DomEventBind('click', '.notifications-link')],
            [[ria.dom.Dom, ria.dom.Event]],
            function notificationsClick(node, event){
                this.dom.find('.new-notification-count').remove();
            },

            function prepareSelect(node){
                var value = parseInt(node.getValue());
                if(!value || value < 1){
                    node.setValue('');
                    node.addClass('prepared');
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
            VOID, function chengeSelect(node, event, selected_){
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
                            this.dom.find('.date-range-popup').removeClass('hidden')
                        else{
                            this.dom.find('.start-end-picker').next().setValue('');
                            this.dom.find('#sort-submit').trigger('click');
                        }

                    }.bind(this), 10);

                node.removeClass('prepared')
            },

            [ria.mvc.DomEventBind('change', '.submit-after-select')],
            [[ria.dom.Dom, ria.dom.Event, Object]],
            VOID, function submitAfterSelect(node, event, selected_){
                this.dom.find('#sort-submit').trigger('click');
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

            function sortSubmit(){
                this.dom.find('#sort-submit').trigger('click');
                var select = this.dom.find('#sort-select');
                select.removeClass('chosen-container-active');
                select.removeClass('chosen-with-drop');
            },

            [ria.mvc.DomEventBind('mousedown', '#sort-select .chosen-drop')],
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
            },

            function $() {
                BASE();
                this._dropDownClicked = false;
            },

            OVERRIDE, VOID, function onRender_(model) {
                BASE(model);
                var that = this;
                new ria.dom.Dom().on('click.dates', function (node, event) {
                    var target = new ria.dom.Dom(event.target), dom = that.dom, popup = dom.find('.date-range-popup');
                    if (!popup.hasClass('hidden') && !target.isOrInside('.date-range-popup') && !target.isOrInside('.ui-datepicker-header') && !target.isOrInside('.ui-datepicker-calendar')) {
                        popup.addClass('hidden');
                        dom.find('.to-set').setValue('false');
                    }
                });
            },

            OVERRIDE, VOID, function onStop_() {
                BASE();
                new ria.dom.Dom().off('click.dates');
            }

        ]);
});