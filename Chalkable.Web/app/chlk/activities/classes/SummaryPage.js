REQUIRE('chlk.activities.feed.BaseFeedPage');
REQUIRE('chlk.templates.classes.ClassSummary');
REQUIRE('chlk.templates.announcement.FeedItemsTpl');
REQUIRE('chlk.templates.feed.Feed');

NAMESPACE('chlk.activities.classes', function () {

    /** @class chlk.activities.classes.SummaryPage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [chlk.activities.lib.PageClass('profile')],
        [ria.mvc.TemplateBind(chlk.templates.classes.ClassSummary)],
        'SummaryPage', EXTENDS(chlk.activities.feed.BaseFeedPage), [

            Boolean, 'readonly',

            OVERRIDE, function getClassScheduledDays_(model){
                var feed = model.getClazz().getFeed();
                return feed.getClassScheduledDays && feed.getClassScheduledDays() || [];
            },

            [ria.mvc.PartialUpdateRule(chlk.templates.feed.Feed, null)],
            VOID, function feedUpdate(tpl, model, msg_) {
                model.setReadonly(this.isReadonly());
                tpl.setReadonly(this.isReadonly());
                tpl.renderTo(this.dom.find('.feed-stats').setHTML(''));
            },

            [ria.mvc.PartialUpdateRule(chlk.templates.announcement.FeedItemsTpl, null)],
            VOID, function feedItemsUpdate(tpl, model, msg_) {
                model.setReadonly(this.isReadonly());
                tpl.setReadonly(this.isReadonly());
                tpl.renderTo(this.dom.find('.chlk-grid'));
            },

            [[Object]],
            OVERRIDE, VOID, function onRefresh_(model) {
                BASE(model);
                this.setReadonly(model.getClazz().getFeed().isReadonly());
            }

        ]);
});