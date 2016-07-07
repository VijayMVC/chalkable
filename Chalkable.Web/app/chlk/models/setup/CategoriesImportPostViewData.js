REQUIRE('ria.serialize.SJX');

NAMESPACE('chlk.models.setup', function () {
    "use strict";

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.setup.CategoriesImportPostViewData*/
    CLASS('CategoriesImportPostViewData', IMPLEMENTS(ria.serialize.IDeserializable), [

        VOID, function deserialize(raw) {
            this.itemsToCopy = SJX.fromValue(raw.itemsToCopy, String);
            this.toClassId = SJX.fromValue(raw.toClassId, chlk.models.id.ClassId);
            this.classId = SJX.fromValue(raw.classId, chlk.models.id.ClassId);
            this.submitType = SJX.fromValue(raw.submitType, String);
        },

        chlk.models.id.ClassId, 'toClassId',

        chlk.models.id.ClassId, 'classId',

        String, 'itemsToCopy',

        String, 'submitType'
    ]);
});
