REQUIRE('chlk.models.common.NameId');

NAMESPACE('chlk.models.settings', function () {
    "use strict";

    /** @class chlk.models.settings.AddSchoolToReportCardsViewData*/
    CLASS('AddSchoolToReportCardsViewData', [

        ArrayOf(chlk.models.common.NameId), 'schools',
        String, 'requestId',
        Array, 'excludeIds',

        function $(schools_, excludeIds_, requestId_){
            BASE();
            schools_ && this.setSchools(schools_);
            excludeIds_ && this.setExcludeIds(excludeIds_);
            requestId_ && this.setRequestId(requestId_);
        }
    ]);
});
