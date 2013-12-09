NAMESPACE('chlk.models.people', function () {
    "use strict";
    /** @class chlk.models.people.Claim*/
    CLASS('Claim',  [
        String, 'type',
        ArrayOf(String), 'values'
    ]);
});
