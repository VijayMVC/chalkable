NAMESPACE('chlk.models.school', function () {
    "use strict";
    /** @class chlk.models.school.SchoolPeoplePart*/
    CLASS(
        'SchoolPeoplePart', [
            chlk.models.common.PaginatedList, 'users',
            Boolean, 'byLastName',
            Number, 'selectedIndex',
            Number, 'schoolId',
            Number, 'roleId',
            Number, 'gradeLevelId',
            Boolean, 'byLastName'
        ]);
});
