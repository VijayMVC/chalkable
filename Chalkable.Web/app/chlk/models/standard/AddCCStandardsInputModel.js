REQUIRE('chlk.models.id.AppId');
REQUIRE('chlk.models.id.ABStandardId');

NAMESPACE('chlk.models.standard', function(){

    /**@class chlk.models.standard.AddCCStandardsInputModel*/
    CLASS('AddCCStandardsInputModel', [

        String, 'requestId',

        READONLY, ArrayOf(chlk.models.id.ABStandardId), 'standardsIds',

        ArrayOf(chlk.models.id.ABStandardId), function getStandardsIds(){
            return this.getStandardsIdsStr()
                ? this.getStandardsIdsStr().split(',')
                    .map(function(item){
                        return new chlk.models.id.ABStandardId(item);
                    })
                : [];
        },

        String, 'standardsIdsStr',

        [ria.serialize.SerializeProperty('standardid')],
        chlk.models.id.ABStandardId, 'standardId'
    ]);
});
