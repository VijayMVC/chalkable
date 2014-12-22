REQUIRE('chlk.models.id.AnnouncementTypeGradingId');

NAMESPACE('chlk.models.grading', function () {
    "use strict";

     /** @class chlk.models.grading.AnnouncementTypeFinal*/
    CLASS(
        'AnnouncementTypeFinal', [
            Boolean, 'dropLowest',

            Number, 'gradingStyle',

            chlk.models.id.AnnouncementTypeGradingId, 'finalGradeAnnouncementTypeId',

            Number, 'percentValue',

            chlk.models.id.FinalGradeId, 'finalGradeId'
        ]);
});
