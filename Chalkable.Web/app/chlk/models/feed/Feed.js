REQUIRE('chlk.models.announcement.Announcement');
REQUIRE('chlk.models.common.PageWithClasses');


NAMESPACE('chlk.models.feed', function () {
    "use strict";

    /** @class chlk.models.feed.Feed*/
    CLASS(
        'Feed', EXTENDS(chlk.models.common.PageWithClasses), [
            [[ArrayOf(chlk.models.announcement.Announcement), chlk.models.classes.ClassesForTopBar, Boolean, Number]],
            function $(items_, classes_, completeOnly_, newNotificationCount_){
                BASE(classes_);
                if(items_)
                    this.setItems(items_);
                if(completeOnly_)
                    this.setCompleteOnly(completeOnly_);
                if(newNotificationCount_)
                    this.setNewNotificationCount(newNotificationCount_);
            },

            ArrayOf(chlk.models.announcement.Announcement), 'items',

            Boolean, 'completeOnly',

            Number, 'importantCount',

            Number, 'newNotificationCount'
        ]);
});