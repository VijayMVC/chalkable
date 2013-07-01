REQUIRE('chlk.models.school.SchoolDetails');

NAMESPACE('chlk.models.school', function () {
    "use strict";
    /** @class chlk.models.school.SchoolPeopleSummary*/
    CLASS(
        'SchoolPeopleSummary', EXTENDS(chlk.models.school.SchoolDetails),[
            Number, "invitesCount",
            Number, "staffCount",
            Number, "studentsCount",
            Number, "teachersCount"
        ]);
});
