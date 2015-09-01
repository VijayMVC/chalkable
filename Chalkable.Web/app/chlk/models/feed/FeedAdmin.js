REQUIRE('chlk.models.common.PageWithGrades');
REQUIRE('chlk.models.announcement.FeedAnnouncementViewData');


NAMESPACE('chlk.models.feed', function () {
    "use strict";

    /** @class chlk.models.feed.FeedAdmin*/
    CLASS(
        'FeedAdmin', EXTENDS(chlk.models.common.PageWithGrades), [
            [[ArrayOf(chlk.models.announcement.FeedAnnouncementViewData), chlk.models.grading.GradeLevelsForTopBar, Boolean, Number]],
            function $(items_, gradeLevels_, importantOnly_, newNotificationCount_){
                BASE(gradeLevels_);
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