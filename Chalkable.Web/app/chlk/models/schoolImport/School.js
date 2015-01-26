REQUIRE('chlk.models.schoolImport.SchoolYear');

NAMESPACE('chlk.models.schoolImport', function () {
    "use strict";
    /** @class chlk.models.schoolImport.School*/
    CLASS(
        'School', [
            String, 'address',
            String, 'city',
            String, 'email',
            String, 'name',
            String, 'phone',
            [ria.serialize.SerializeProperty('principalemail')],
            String, 'principalEmail',
            [ria.serialize.SerializeProperty('principalname')],
            String, 'principalName',
            [ria.serialize.SerializeProperty('principaltitle')],
            String, 'principalTitle',
            [ria.serialize.SerializeProperty('schoolid')],
            Number, 'sisSchoolId',
            [ria.serialize.SerializeProperty('schoolyears')],
            ArrayOf(chlk.models.schoolImport.SchoolYear), 'schoolYears'
        ]);
});
