REQUIRE('ria.serialize.SJX');
REQUIRE('ria.serialize.IDeserializable');
REQUIRE('chlk.models.id.AnnouncementAttributeId');



NAMESPACE('chlk.models.announcement', function(){

    var SJX = ria.serialize.SJX;

    /**@class chlk.models.announcement.AnnouncementAttributeType*/
    CLASS(
        UNSAFE, FINAL, 'AnnouncementAttributeType', IMPLEMENTS(ria.serialize.IDeserializable), [

        chlk.models.id.AnnouncementAttributeId, 'id',

        String, 'code',

        String, 'name',

        String, 'description',

        VOID, function deserialize(raw) {
            this.id = SJX.fromValue(raw.id, chlk.models.id.AnnouncementAttributeId);
            this.code = SJX.fromValue(raw.code, String);
            this.name = SJX.fromValue(raw.name, String);
            this.description = SJX.fromValue(raw.description, String);
        }

    ]);
});