NAMESPACE('chlk.models.announcement', function () {
    "use strict";

    /** @class chlk.models.announcement.AnnouncementTypeEnum*/
    ENUM(
        'AnnouncementTypeEnum', {
            CUSTOM : 0,
            ANNOUNCEMENT : 1,
            HW : 2,
            ESSAY : 3,
            QUIZ : 4,
            TEST : 5,
            PROJECT : 6,
            FINAL : 7,
            MIDTERM : 8,
            BOOK_REPORT : 9,
            TERM_PAPER : 10,
            ADMIN : 11
        });

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
