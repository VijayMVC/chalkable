REQUIRE('chlk.models.announcement.ShortStudentAnnouncementViewData');
REQUIRE('chlk.models.announcement.BaseStudentAnnouncementsViewData');

NAMESPACE('chlk.models.announcement', function () {
    "use strict";

    /** @class chlk.models.announcement.ShortStudentAnnouncementsViewData*/
    CLASS(
        'ShortStudentAnnouncementsViewData', EXTENDS(chlk.models.announcement.BaseStudentAnnouncementsViewData), [
            ArrayOf(chlk.models.announcement.ShortStudentAnnouncementViewData), 'items',

            function $(items_){
                BASE();
                if(items_)
                    this.setItems(items_);
            }
        ]);
});
