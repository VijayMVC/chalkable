REQUIRE('chlk.models.id.AppId');


NAMESPACE('chlk.models.standard', function(){

    /**@class chlk.models.standard.AddCCStandardsInputModel*/
    CLASS('AddCCStandardsInputModel', [

        chlk.models.id.AppId, 'applicationId',

        READONLY, Array, 'standardsIds',

        Array, function getStandardsIds(){
            return this.getStandardsIdsStr() ? this.getStandardsIdsStr().split(',') : [];
        },

        String, 'standardsIdsStr'
    ]);
});