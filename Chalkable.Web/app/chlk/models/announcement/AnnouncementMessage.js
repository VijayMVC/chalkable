REQUIRE('chlk.models.people.User');
REQUIRE('chlk.models.common.ChlkDate');

NAMESPACE('chlk.models.announcement', function () {
    "use strict";

    /** @class chlk.models.announcement.AnnouncementMessage*/
    CLASS(
        'AnnouncementMessage', [
            chlk.models.common.ChlkDate, 'created',
            chlk.models.people.User, 'person',
            String, 'message'
        ]);
});
