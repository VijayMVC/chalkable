REQUIRE('chlk.models.announcement.AnnouncementType');
REQUIRE('chlk.models.id.ClassId');

NAMESPACE('chlk.models.announcement', function () {
    "use strict";

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.announcement.CategoriesSubmitViewData*/
    CLASS(
        UNSAFE, 'CategoriesSubmitViewData', IMPLEMENTS(ria.serialize.IDeserializable), [

            VOID, function deserialize(raw){
                this.ids = SJX.fromValue(raw.ids, String);
                this.submitType = SJX.fromValue(raw.submitType, String);
                this.copyToYearName = SJX.fromValue(raw.copyToYearName, String);
                this.toClassName = SJX.fromValue(raw.toClassName, String);
                this.classId = SJX.fromValue(raw.classid, chlk.models.id.ClassId);
                this.toClassId = SJX.fromValue(raw.toClassId, chlk.models.id.ClassId);
            },

            String, 'ids',

            chlk.models.id.ClassId, 'classId',

            chlk.models.id.ClassId, 'toClassId',

            String, 'copyToYearName',

            String, 'toClassName',

            String, 'submitType'
        ]);
});
