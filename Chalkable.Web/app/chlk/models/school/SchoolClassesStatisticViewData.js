REQUIRE('chlk.models.id.DepartmentId');
REQUIRE('chlk.models.id.SchoolYearId');
REQUIRE('chlk.models.admin.BaseStatistic');
REQUIRE('ria.serialize.SJX');

NAMESPACE('chlk.models.school', function () {
    "use strict";

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.school.SchoolClassesStatisticViewData*/
    CLASS(
        'SchoolClassesStatisticViewData', EXTENDS(chlk.models.admin.BaseStatistic), IMPLEMENTS(ria.serialize.IDeserializable), [
            chlk.models.id.DepartmentId, 'departmentId',

            chlk.models.id.SchoolYearId, 'schoolYearId',

            String, 'primaryTeacherName',

            Boolean, 'disciplinesProfileEnabled',

            Boolean, 'attendancesProfileEnabled',

            Boolean, 'gradingProfileEnabled',

            String, 'classNumber',

            String, 'periods',

            ArrayOf(chlk.models.id.SchoolPersonId), 'teachersIds',

            function getNoAccessMsg(name){
                return 'User does not have permission to access ' + name + ' for section \'' + this.getId().valueOf() + '\'';
            },

            OVERRIDE, VOID, function deserialize(raw){
                BASE(raw);
                this.departmentId = SJX.fromValue(raw.departmentref, chlk.models.id.DepartmentId);
                this.primaryTeacherName = SJX.fromValue(raw.primaryteacherdisplayname, String);
                this.classNumber = SJX.fromValue(raw.classnumber, String);
                this.periods = SJX.fromValue(raw.periods, String);
                this.teachersIds = SJX.fromArrayOfValues(raw.teacherids || [], chlk.models.id.SchoolPersonId);
            }
        ]);
});
