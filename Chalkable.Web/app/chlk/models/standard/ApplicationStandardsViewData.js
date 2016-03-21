REQUIRE('chlk.models.academicBenchmark.Standard');
REQUIRE('chlk.models.id.AppId');


NAMESPACE('chlk.models.standard', function(){

    /**@class chlk.models.standard.ApplicationStandardsViewData*/
    CLASS('ApplicationStandardsViewData', [

        ArrayOf(chlk.models.academicBenchmark.Standard), 'standards',

        chlk.models.id.AppId, 'applicationId',
        Boolean, 'readOnly',

        [[chlk.models.id.AppId, ArrayOf(chlk.models.academicBenchmark.Standard), Boolean]],
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