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

            OVERRIDE, VOID, function onRender_(model) {
                BASE(model);

                //ria.dom.Dom('.markDoneSelect').setValue(0);
            },

        ]);
});