REQUIRE('ria.serialize.SJX');
REQUIRE('ria.serialize.IDeserializable');
REQUIRE('chlk.models.standard.Standard');
REQUIRE('chlk.models.standard.StandardGrading');

NAMESPACE('chlk.models.standard', function () {
    "use strict";

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.standard.StandardForExplorer*/
    CLASS(UNSAFE, FINAL, 'StandardForExplorer', EXTENDS(chlk.models.standard.Standard), IMPLEMENTS(ria.serialize.IDeserializable),  [
        chlk.models.standard.StandardGrading, 'standardGrading',

        OVERRIDE, VOID, function deserialize(raw) {
            BASE(raw);
            this.standardGrading = SJX.fromDeserializable(raw.standardgrading, chlk.models.standard.StandardGrading);
        }
    ]);
});
