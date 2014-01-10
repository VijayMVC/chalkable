REQUIRE('chlk.models.announcement.Announcement');
REQUIRE('chlk.models.common.PageWithClasses');


NAMESPACE('chlk.models.feed', function () {
    "use strict";

    /** @class chlk.models.feed.Feed*/
    CLASS(
        'Feed', EXTENDS(chlk.models.common.PageWithClasses), [
            [[ArrayOf(chlk.models.announcement.Announcement), chlk.models.classes.ClassesForTopBar, Boolean]],
            function $(items_, classes_, starredOnly_){
                BASE(classes_);
                if(items_)
                    this.setItems(items_);
                if(starredOnly_)
                    this.setStarredOnly(starredOnly_);
            },

            ArrayOf(chlk.models.announcement.Announcement), 'items',

            Boolean, 'starredOnly',

            Number, 'importantCount'
        ]);
});