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

            function $(){
                BASE();
                this._description = null;
            },

            [ria.serialize.SerializeProperty('cancreate')],
            Boolean, 'canCreate',
            String, 'description',
            [[String]],
            VOID, function setDescription(description){
                this._description = description;
            },
            String, function getDescription(){
                return this._description || this.getName()
            },

            [ria.serialize.SerializeProperty('announcementtypeid')],
            Number, 'announcementTypeId',

            [ria.serialize.SerializeProperty('issystem')],
            Boolean, 'isSystem',
            String, 'name'
        ]);
});
