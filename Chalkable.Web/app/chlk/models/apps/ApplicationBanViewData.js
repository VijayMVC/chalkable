REQUIRE('chlk.models.id.AppId');
REQUIRE('chlk.models.apps.ApplicationSchoolBan');

NAMESPACE('chlk.models.apps', function () {
    "use strict";

    /** @class chlk.models.apps.ApplicationBanViewData*/
    CLASS(
        'ApplicationBanViewData', [

            chlk.models.id.AppId, 'applicationId',
            String, 'requestId',
            ArrayOf(chlk.models.apps.ApplicationSchoolBan), 'schools',

            [[
                String,
                chlk.models.id.AppId,
                ArrayOf(chlk.models.apps.ApplicationSchoolBan)
            ]],
            function $(requestId_, applicationId_, schools_){
                BASE();
                if(requestId_)
                    this.setRequestId(requestId_);
                if(applicationId_)
                    this.setApplicationId(applicationId_);
                if(schools_)
                    this.setSchools(schools_);

            }
        ]);

    /** @class chlk.models.apps.SubmitApplicationBan*/
    CLASS('SubmitApplicationBan',[

        chlk.models.id.AppId, 'applicationId',
        String, 'requestId',
        String, 'schoolIdsStr',

        ArrayOf(chlk.models.id.SchoolId), function getSchoolIds(){
            var idsStr = this.getSchoolIdsStr();
            return idsStr ? idsStr.split(',').map(function(_){return new chlk.models.id.SchoolId(_);}) : [];
        }
    ])
});