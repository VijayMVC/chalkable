REQUIRE('chlk.models.schoolYear.Year');
REQUIRE('chlk.models.classes.Class');
REQUIRE('ria.serialize.SJX');

NAMESPACE('chlk.models.schoolYear', function () {
    "use strict";

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.schoolYear.YearAndClasses*/
    CLASS(
        'YearAndClasses', IMPLEMENTS(ria.serialize.IDeserializable), [

            VOID, function deserialize(raw) {
                this.schoolYear = SJX.fromDeserializable(raw.schoolyear, chlk.models.schoolYear.Year);
                this.classes = SJX.fromArrayOfDeserializables(raw.classes, chlk.models.classes.Class);
            },

            chlk.models.schoolYear.Year, 'schoolYear',
            ArrayOf(chlk.models.classes.Class), 'classes'
        ]);
});
