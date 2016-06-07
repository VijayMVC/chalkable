REQUIRE('chlk.models.announcement.AnnouncementForm');
REQUIRE('chlk.models.people.User');

NAMESPACE('chlk.models.announcement', function () {
    "use strict";

    /** @class chlk.models.announcement.SupplementalAnnouncementForm*/
    CLASS(
        'SupplementalAnnouncementForm', EXTENDS(chlk.models.announcement.AnnouncementForm), [
            ArrayOf(chlk.models.people.User), 'students'
        ]);
});
