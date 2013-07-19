NAMESPACE('chlk.models.announcement', function () {
    "use strict";

    /** @class chlk.models.announcement.AnnouncementType*/
    CLASS(
        'AnnouncementType', [
            [ria.serialize.SerializeProperty('cancreate')],
            Boolean, 'canCreate',
            String, 'description',
            Number, 'id',
            [ria.serialize.SerializeProperty('issystem')],
            Boolean, 'isSystem',
            String, 'name'
        ]);
});
