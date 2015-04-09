REQUIRE('chlk.models.id.AppId');
REQUIRE('chlk.models.id.CommonCoreStandardId');

NAMESPACE('chlk.models.standard', function(){

    /**@class chlk.models.standard.AddCCStandardsInputModel*/
    CLASS('AddCCStandardsInputModel', [

        chlk.models.id.AppId, 'applicationId',

        READONLY, ArrayOf(chlk.models.id.CommonCoreStandardId), 'standardsIds',

        ArrayOf(chlk.models.id.CommonCoreStandardId), function getStandardsIds(){
            return this.getStandardsIdsStr()
                ? this.getStandardsIdsStr().split(',')
                    .map(function(item){
                        return new chlk.models.id.CommonCoreStandardId(item);
                    })
                : [];
        },

        String, 'standardsIdsStr',

        [ria.serialize.SerializeProperty('standardid')],
        chlk.models.id.CommonCoreStandardId, 'standardId'
    ]);
});