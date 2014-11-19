REQUIRE('chlk.models.standard.CommonCoreStandard');
REQUIRE('chlk.models.id.AppId');


NAMESPACE('chlk.models.standard', function(){

    /**@class chlk.models.standard.ApplicationStandardsViewData*/
    CLASS('ApplicationStandardsViewData', [

        ArrayOf(chlk.models.standard.CommonCoreStandard), 'standards',

        chlk.models.id.AppId, 'applicationId',

        [[chlk.models.id.AppId, ArrayOf(chlk.models.standard.CommonCoreStandard)]],
        function $(applicationId,  standards){
            BASE();
            if(applicationId)
                this.setApplicationId(applicationId);
            if(standards)
                this.setStandards(standards);
        }
    ]);
});