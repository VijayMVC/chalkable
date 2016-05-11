REQUIRE('chlk.models.id.AppId');
REQUIRE('chlk.models.id.SchoolId');
REQUIRE('chlk.models.school.School');


NAMESPACE('chlk.models.apps', function () {
    "use strict";

    /** @class chlk.models.apps.ApplicationBanViewData*/
    CLASS(
        'ApplicationBanViewData', [

            chlk.models.id.AppId, 'applicationId',
            String, 'requestId',
            ArrayOf(chlk.models.id.SchoolId), 'bannedSchoolIds',
            ArrayOf(chlk.models.school.School), 'schools',
            Boolean, 'banned',

            [[
                chlk.models.id.AppId,
                String,
                ArrayOf(chlk.models.id.SchoolId),
                ArrayOf(chlk.models.school.School),
                Boolean
            ]],
            function $(applicationId_, requestId_, bannedSchoolIds_, schools_, banned_){
                BASE();
                if(applicationId_)
                    this.setApplicationId(applicationId_);
                if(requestId_)
                    this.setRequestId(requestId_);
                if(bannedSchoolIds_)
                    this.setBannedSchoolIds(bannedSchoolIds_);
                if(schools_)
                    this.setSchools(schools_);
                if(banned_)
                    this.setBanned(banned_);
            }
        ]);
});