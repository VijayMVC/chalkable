REQUIRE('chlk.models.people.Person');
REQUIRE('chlk.models.common.ChlkDate');

NAMESPACE('chlk.models.announcement', function () {
    "use strict";

    /** @class chlk.models.announcement.AnnouncementMessage*/
    CLASS(
        'AnnouncementMessage', [
            chlk.models.common.ChlkDate, 'created',
            chlk.models.people.Person, 'person',
            String, 'message'
        ]);
});
