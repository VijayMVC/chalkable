REQUIRE('ria.serialize.SJX');
REQUIRE('ria.serialize.IDeserializable');
REQUIRE('chlk.models.id.AppCategoryId');

NAMESPACE('chlk.models.apps', function () {
    "use strict";

    var SJX = ria.serialize.SJX;
    /** @class chlk.models.apps.AppCategory*/
    CLASS(
        FINAL, UNSAFE, 'AppCategory', IMPLEMENTS(ria.serialize.IDeserializable),  [
            VOID, function deserialize(raw){
                this.id = SJX.fromValue(raw.id, chlk.models.id.AppCategoryId);
                this.name = SJX.fromValue(raw.name, String);
                this.description = SJX.fromValue(raw.description, String);
            },
            chlk.models.id.AppCategoryId, 'id',
            String, 'name',
            String, 'description'
        ]);
});
