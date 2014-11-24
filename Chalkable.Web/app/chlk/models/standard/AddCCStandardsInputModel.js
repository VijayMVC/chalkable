REQUIRE('chlk.models.id.AppId');


NAMESPACE('chlk.models.standard', function(){

    /**@class chlk.models.standard.AddCCStandardsInputModel*/
    CLASS('AddCCStandardsInputModel', [

        chlk.models.id.AppId, 'applicationId',

        READONLY, Array, 'standardCodes',

        Array, function getStandardCodes(){
            return this.getStandardCodesStr() ? this.getStandardCodesStr().split(',') : [];
        },

        String, 'standardCodesStr'
    ]);
});