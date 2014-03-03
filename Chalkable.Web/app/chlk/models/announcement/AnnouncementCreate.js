REQUIRE('chlk.models.common.ChlkDate');
REQUIRE('chlk.models.announcement.Announcement');

NAMESPACE('chlk.models.announcement', function () {
    "use strict";

    /** @class chlk.models.announcement.AnnouncementCreate*/
    CLASS(
        'AnnouncementCreate', [
            chlk.models.announcement.Announcement, 'announcement',

            [ria.serialize.SerializeProperty('isdraft')],
            Boolean, 'isDraft',

            [ria.serialize.SerializeProperty('canaddstandard')],
            Boolean, 'canAddStandard',

            Array, 'recipients'
        ]);
});
