REQUIRE('ria.serialize.SJX');

REQUIRE('chlk.models.id.LpGalleryCategoryId');

NAMESPACE('chlk.models.announcement', function(){

    var SJX = ria.serialize.SJX;

    /**@class chlk.models.announcement.CategoryViewData*/
    CLASS('CategoryViewData', IMPLEMENTS(ria.serialize.IDeserializable),[
        VOID, function deserialize(raw){
            this.id = SJX.fromValue(raw.id, chlk.models.id.LpGalleryCategoryId);
            this.name = SJX.fromValue(raw.name, String);
            this.lessonPlansCount = SJX.fromValue(raw.lessonplanscount, Number);
        },

        Number, 'lessonPlansCount',
        chlk.models.id.LpGalleryCategoryId, 'id',
        String, 'name'
    ]);
});