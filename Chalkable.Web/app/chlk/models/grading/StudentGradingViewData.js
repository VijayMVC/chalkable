REQUIRE('chlk.models.people.User');
REQUIRE('ria.serialize.SJX');
REQUIRE('ria.serialize.IDeserializable');

NAMESPACE('chlk.models.grading', function () {
    "use strict";

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.grading.StudentWellTroubleEnum*/
    ENUM('StudentWellTroubleEnum', {
        NORMALL: 0,
        WELL: 1,
        TROUBLE: 2
    });

    /** @class chlk.models.grading.StudentGradingViewData*/
    CLASS(
        UNSAFE, 'StudentGradingViewData', EXTENDS(chlk.models.people.User), IMPLEMENTS(ria.serialize.IDeserializable), [
            OVERRIDE, VOID, function deserialize(raw){
                BASE(raw);
                this.avg = SJX.fromValue(raw.avg, Number);
            },

            Number, 'avg',
            Number, 'right',
            chlk.models.grading.StudentWellTroubleEnum, 'wellTroubleType'
        ]);
});
