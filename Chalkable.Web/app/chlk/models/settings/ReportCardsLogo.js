REQUIRE('chlk.models.schoolYear.GradingPeriod');
REQUIRE('chlk.models.id.ReportCardsLogoId');

NAMESPACE('chlk.models.settings', function (){
    "use strict";

    /**@class chlk.models.settings.ReportCardsLogo*/
    CLASS('ReportCardsLogo', [

        [ria.serialize.SerializeProperty('id')],
        chlk.models.id.ReportCardsLogoId, 'id',

        [ria.serialize.SerializeProperty('schoolid')],
        Number, 'schoolId',

        [ria.serialize.SerializeProperty('schoolname')],
        String, 'schoolName',

        [ria.serialize.SerializeProperty('logoaddress')],
        String, 'logoAddress',

        [ria.serialize.SerializeProperty('districtlogo')],
        Boolean, 'districtLogo'
    ]);
});