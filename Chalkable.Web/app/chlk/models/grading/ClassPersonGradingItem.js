REQUIRE('chlk.models.announcement.ClassAnnouncementType');
REQUIRE('chlk.models.profile.ShortAnnouncementForProfileViewData');
REQUIRE('chlk.models.Popup');

NAMESPACE('chlk.models.grading', function () {
    "use strict";

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.grading.ClassPersonGradingItem*/
    CLASS(
        UNSAFE, 'ClassPersonGradingItem', EXTENDS(chlk.models.Popup), IMPLEMENTS(ria.serialize.IDeserializable), [

            Number, 'studentItemTypeAvg',

            chlk.models.announcement.ClassAnnouncementType, 'announcementType',

            ArrayOf(chlk.models.profile.ShortAnnouncementForProfileViewData), 'announcements',

            VOID, function deserialize(raw){
                this.studentItemTypeAvg = SJX.fromValue(raw.avg, Number);
                this.announcementType = SJX.fromDeserializable(raw.announcementtype, chlk.models.announcement.ClassAnnouncementType);
                this.announcements = SJX.fromArrayOfDeserializables(raw.items, chlk.models.profile.ShortAnnouncementForProfileViewData);
            }

        ]);
});
