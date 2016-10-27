REQUIRE('ria.serialize.SJX');
REQUIRE('ria.serialize.IDeserializable');
REQUIRE('chlk.models.standard.Standard');

NAMESPACE('chlk.models.standard', function () {
    "use strict";

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.standard.StandardForClassExplorer*/
    CLASS(UNSAFE, FINAL, 'StandardForClassExplorer', EXTENDS(chlk.models.standard.Standard), IMPLEMENTS(ria.serialize.IDeserializable),  [
        Number, 'numericGrade',

        OVERRIDE, VOID, function deserialize(raw) {
            BASE(raw);
            this.numericGrade = SJX.fromValue(raw.numericgrade, Number);
        }
    ]);
});
