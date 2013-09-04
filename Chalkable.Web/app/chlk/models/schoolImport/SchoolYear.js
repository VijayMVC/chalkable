REQUIRE('chlk.models.common.ChlkDate');
NAMESPACE('chlk.models.schoolImport', function () {
    "use strict";
    /** @class chlk.models.schoolImport.SchoolYear*/
    CLASS(
        'SchoolYear', [
            String, 'name',
            [ria.serialize.SerializeProperty('calendarid')],
            Number, 'calendarId',
            [ria.serialize.SerializeProperty('schoolid')],
            Number, 'sisSchoolId',
            [ria.serialize.SerializeProperty('startdate')],
            chlk.models.common.ChlkDate, 'startDate',
            [ria.serialize.SerializeProperty('enddate')],
            chlk.models.common.ChlkDate, 'endDate'
        ]);
});
