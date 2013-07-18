NAMESPACE('chlk.models.announcement', function () {
    "use strict";

    /** @class chlk.models.announcement.AnnouncementTypeId*/
    IDENTIFIER('AnnouncementTypeId');

    /** @class chlk.models.announcement.AnnouncementType*/
    CLASS(
        'AnnouncementType', [
            Boolean, 'cancreate',
            String, 'description',
            chlk.models.announcement.AnnouncementTypeId, 'id',
            Boolean, 'issystem',
            String, 'name'
        ]);
});
