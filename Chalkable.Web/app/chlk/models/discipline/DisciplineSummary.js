REQUIRE('chlk.models.people.User');

NAMESPACE('chlk.models.discipline', function(){
   "use strict";

    /** @class chlk.models.discipline.DisciplineSummary*/
    CLASS(
       'DisciplineSummary', [
            chlk.models.people.User, 'student',

            Number, 'total',

            [ria.serialize.SerializeProperty('discplinerecordsnumber')],
            Number, 'recordsnumber',

            String, 'summary'
        ]);
});