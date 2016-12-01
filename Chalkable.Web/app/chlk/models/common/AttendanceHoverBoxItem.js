REQUIRE('chlk.converters.attendance.AttendanceTypeToNameConverter');
REQUIRE('ria.serialize.SJX');

NAMESPACE('chlk.models.common', function () {
    "use strict";

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.common.AttendanceHoverBoxItem*/
    CLASS(
        UNSAFE, 'AttendanceHoverBoxItem', IMPLEMENTS(ria.serialize.IDeserializable),[

            Number, 'absences',
            chlk.models.classes.Class, 'clazz',

            VOID, function deserialize(raw){
                this.clazz = SJX.fromDeserializable(raw.class, chlk.models.classes.Class);
                this.absences = SJX.fromValue(raw.absences, Number);
            }
        ]);
});
