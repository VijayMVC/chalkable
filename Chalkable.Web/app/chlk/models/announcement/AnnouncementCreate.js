REQUIRE('chlk.models.common.ChlkDate');
REQUIRE('chlk.models.announcement.FeedAnnouncementViewData');
REQUIRE('chlk.models.people.User');

NAMESPACE('chlk.models.announcement', function () {
    "use strict";

    /** @class chlk.models.announcement.AnnouncementCreate*/
    CLASS(
        'AnnouncementCreate', [
            chlk.models.announcement.FeedAnnouncementViewData, 'announcement',

            [ria.serialize.SerializeProperty('isdraft')],
            Boolean, 'isDraft',
            ArrayOf(chlk.models.people.User), 'students',

            function $(announcement_){
                BASE();
                if(announcement_)
                    this.setAnnouncement(announcement_);
            }
        ]);
});
