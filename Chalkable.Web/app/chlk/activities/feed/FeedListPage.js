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

            [ria.mvc.DomEventBind('change', '.markDoneSelect')],
            [[ria.dom.Dom, ria.dom.Event, Object]],
            VOID, function markDoneSelect(node, event, selected_){
                this.dom.find('#mark-done-submit').trigger('click');
            },

            [ria.mvc.DomEventBind('click', '#sort-select-input')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function sortClick(node, event){
                var select = this.dom.find('#sort-select');
                select.toggleClass('chosen-container-active');
                select.toggleClass('chosen-with-drop');
            },

            [ria.mvc.DomEventBind('click', '.latest-earliest-option')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function sortOptionClick(node, event){
                this.dom.find('.latest-earliest-input').setValue(!!node.getData('value'));
                this.sortSubmit();
            },

            [ria.mvc.DomEventBind('change', '.lessonplans-check, .start-end-picker')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function lessonPlansChange(node, event){
                this.sortSubmit();
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
            }

        ]);
});