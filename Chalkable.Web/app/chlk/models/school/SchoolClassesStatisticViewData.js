REQUIRE('chlk.models.id.DepartmentId');
REQUIRE('chlk.models.id.SchoolId');
REQUIRE('chlk.models.admin.BaseStatistic');
REQUIRE('ria.serialize.SJX');

NAMESPACE('chlk.models.school', function () {
    "use strict";

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.school.SchoolClassesStatisticViewData*/
    CLASS(
        'SchoolClassesStatisticViewData', EXTENDS(chlk.models.admin.BaseStatistic), IMPLEMENTS(ria.serialize.IDeserializable), [
            chlk.models.id.DepartmentId, 'departmentId',

            chlk.models.id.SchoolId, 'schoolId',

            String, 'primaryTeacherName',

            OVERRIDE, VOID, function deserialize(raw){
                BASE(raw);
                this.departmentId = SJX.fromValue(raw.departmentref, chlk.models.id.DepartmentId);
                this.primaryTeacherName = SJX.fromValue(raw.primaryteacherref, String);
            }
        ]);
});
