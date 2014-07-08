REQUIRE('chlk.models.common.PageWithClasses');
REQUIRE('chlk.models.announcement.FeedAnnouncementViewData');


NAMESPACE('chlk.models.feed', function () {
    "use strict";

    /** @class chlk.models.feed.Feed*/
    CLASS(
        'Feed', EXTENDS(chlk.models.common.PageWithClasses), [
            [[ArrayOf(chlk.models.announcement.FeedAnnouncementViewData), chlk.models.classes.ClassesForTopBar, Boolean, Number]],
            function $(items_, classes_, importantOnly_, newNotificationCount_){
                BASE(classes_);
                if(items_)
                    this.setItems(items_);
                if(importantOnly_)
                    this.setImportantOnly(importantOnly_);
                if(newNotificationCount_)
                    this.setNewNotificationCount(newNotificationCount_);
            },

            ArrayOf(chlk.models.announcement.FeedAnnouncementViewData), 'items',

            Boolean, 'importantOnly',

            Number, 'importantCount',

            Number, 'newNotificationCount'
        ]);
});