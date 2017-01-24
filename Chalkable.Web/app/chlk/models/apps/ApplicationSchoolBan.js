REQUIRE('chlk.models.id.AppId');
REQUIRE('chlk.models.id.SchoolId');


NAMESPACE('chlk.models.apps', function () {
    "use strict";
    /** @class chlk.models.apps.ApplicationSchoolBan*/
    CLASS(
        'ApplicationSchoolBan', [

            [ria.serialize.SerializeProperty('applicationid')],
            chlk.models.id.AppId, 'applicationId',

            [ria.serialize.SerializeProperty('schoolid')],
            chlk.models.id.SchoolId, 'schoolId',

            [ria.serialize.SerializeProperty('schoolname')],
            String, 'schoolName',

            Boolean, 'banned',
    ]);
});