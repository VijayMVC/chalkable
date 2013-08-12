REQUIRE('chlk.models.id.AnnouncementTypeGradingId');
REQUIRE('chlk.models.id.FinalGradeId');

NAMESPACE('chlk.models.grading', function () {
    "use strict";

   /** @class chlk.models.grading.AnnouncementTypeGrading*/
    CLASS(
        'AnnouncementTypeGrading', [
            [ria.serialize.SerializeProperty('droplowest')],
            Boolean, 'dropLowest',

            [ria.serialize.SerializeProperty('gradingstyle')],
            Number, 'gradingStyle',

            chlk.models.id.AnnouncementTypeGradingId, 'id',

            [ria.serialize.SerializeProperty('typename')],
            String, 'typeName',

            Number, 'value',

            Number, 'index'
        ]);
});
