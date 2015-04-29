REQUIRE('chlk.models.announcement.AnnouncementType');
REQUIRE('chlk.models.id.ClassId');

NAMESPACE('chlk.models.announcement', function () {
    "use strict";

    /** @class chlk.models.announcement.ClassAnnouncementType*/
    CLASS(
        'ClassAnnouncementType', EXTENDS(chlk.models.announcement.AnnouncementType), [
            [[Number, String]],
            function $(id_, name_, classId_){
                BASE();
                if(id_)
                    this.setId(id_);
                if(name_)
                    this.setName(name_);
                if(classId_)
                    this.setClassId(classId_);
            },

            Number, 'id',

            String, 'ids',

            [ria.serialize.SerializeProperty('classid')],
            chlk.models.id.ClassId, 'classId',

            [ria.serialize.SerializeProperty('highscorestodrop')],
            Number, 'highScoresToDrop',

            [ria.serialize.SerializeProperty('lowscorestodrop')],
            Number, 'lowScoresToDrop',

            [ria.serialize.SerializeProperty('percentage')],
            Number, 'percentage'
        ]);
});
