REQUIRE('chlk.models.school.SchoolDetails');

NAMESPACE('chlk.models.school', function () {
    "use strict";
    /** @class chlk.models.school.SchoolPeopleSummary*/
    CLASS(
        'SchoolPeopleSummary', EXTENDS(chlk.models.school.SchoolDetails),[
            [ria.serialize.SerializeProperty('invitescount')],
            Number, "invitesCount",
            [ria.serialize.SerializeProperty('staffcount')],
            Number, "staffCount",
            [ria.serialize.SerializeProperty('studentscount')],
            Number, "studentsCount",
            [ria.serialize.SerializeProperty('teacherscount')],
            Number, "teachersCount"
        ]);
});
