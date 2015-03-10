REQUIRE('chlk.models.standard.CommonCoreStandard');
REQUIRE('chlk.models.id.AppId');


NAMESPACE('chlk.models.standard', function(){

    /**@class chlk.models.standard.ApplicationStandardsViewData*/
    CLASS('ApplicationStandardsViewData', [

        ArrayOf(chlk.models.standard.CommonCoreStandard), 'standards',

        chlk.models.id.AppId, 'applicationId',
        Boolean, 'readOnly',

        [[chlk.models.id.AppId, ArrayOf(chlk.models.standard.CommonCoreStandard), Boolean]],
        function $(applicationId,  standards, readOnly_){
            this.setReadOnly(readOnly_ ? readOnly_ : false);
            BASE();
            if(applicationId)
                this.setApplicationId(applicationId);
            if(standards)
                this.setStandards(standards);
        }
    ]);
});