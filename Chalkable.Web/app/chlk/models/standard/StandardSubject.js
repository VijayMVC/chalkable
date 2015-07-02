REQUIRE('chlk.models.id.StandardSubjectId');

NAMESPACE('chlk.models.standard', function () {
    "use strict";
    /** @class chlk.models.standard.StandardSubject*/
    CLASS(
        'StandardSubject', [
           String, 'name',
           String, 'description',
           chlk.models.id.StandardSubjectId, 'id'
        ]);
});
