REQUIRE('chlk.models.announcement.AnnouncementType');
REQUIRE('chlk.models.id.ClassId');

NAMESPACE('chlk.models.announcement', function () {
    "use strict";

    /** @class chlk.models.announcement.ClassAnnouncementType*/
    CLASS(
        'ClassAnnouncementType', EXTENDS(chlk.models.announcement.AnnouncementType), [
            [[Number, String]],
            function $(id_, name_){
                BASE();
                if(id_)
                    this.setId(id_);
                if(name_)
                    this.setName(name_);
            },

            Number, 'id',
            [ria.serialize.SerializeProperty('classid')],
            chlk.models.id.ClassId, 'classId'
        ]);
});
