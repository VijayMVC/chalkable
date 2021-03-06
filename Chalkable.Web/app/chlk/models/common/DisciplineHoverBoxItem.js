REQUIRE('ria.serialize.SJX');
REQUIRE('chlk.models.common.NameId');

NAMESPACE('chlk.models.common', function () {
    "use strict";
    var SJX = ria.serialize.SJX;

    /** @class chlk.models.common.DisciplineHoverBoxItem*/
    CLASS(
        UNSAFE, 'DisciplineHoverBoxItem', IMPLEMENTS(ria.serialize.IDeserializable), [

            Number, 'total',
            chlk.models.common.NameId, 'disciplineType',

            VOID, function deserialize(raw){
                this.total = SJX.fromValue(raw.total, Number);
                this.disciplineType = SJX.fromDeserializable(raw.type, chlk.models.common.NameId);
            }
    ]);
});
