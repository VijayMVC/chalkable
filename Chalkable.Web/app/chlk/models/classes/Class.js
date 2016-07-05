REQUIRE('chlk.models.people.User');
REQUIRE('chlk.models.id.ClassId');
REQUIRE('chlk.models.id.MarkingPeriodId');
REQUIRE('chlk.models.id.DepartmentId');
REQUIRE('chlk.models.id.SchoolPersonId');
REQUIRE('chlk.models.id.SchoolId');
REQUIRE('chlk.models.id.CourseTypeId');
REQUIRE('chlk.models.classes.Room');

NAMESPACE('chlk.models.classes', function () {
    "use strict";

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.classes.DateTypeEnum*/
    ENUM('DateTypeEnum', {
        YEAR: 0,
        GRADING_PERIOD: 1,
        LAST_MONTH: 2,
        LAST_WEEK: 3
    });

    /** @class chlk.models.classes.Class*/
    CLASS(
        UNSAFE, 'Class', IMPLEMENTS(ria.serialize.IDeserializable), [

            VOID, function deserialize(raw){
                this.departmentId = SJX.fromValue(raw.departmentid, chlk.models.id.DepartmentId);
                this.description = SJX.fromValue(raw.description, String);
                this.id = SJX.fromValue(raw.id, chlk.models.id.ClassId);
                this.markingPeriodsId = SJX.fromArrayOfValues(raw.markingperiodsid, chlk.models.id.MarkingPeriodId);
                this.classNumber = SJX.fromValue(raw.classnumber, String);
                this.name = SJX.fromValue(raw.name, String);
                this.teacher = SJX.fromDeserializable(raw.teacher, chlk.models.people.User);
                this.defaultAnnouncementTypeId = SJX.fromValue(raw.defaultAnnouncementTypeId, Number);
                this.teachersIds = SJX.fromArrayOfValues(raw.teachersids, chlk.models.id.SchoolPersonId);
                this.schoolId = raw.schoolyear ? SJX.fromValue(raw.schoolyear.schoolid, chlk.models.id.SchoolId) : null;
                this.courseTypeId = SJX.fromValue(raw.coursetypeid, chlk.models.id.CourseTypeId);
                this.room = SJX.fromDeserializable(raw.room, chlk.models.classes.Room);
                this.periods = SJX.fromArrayOfValues(raw.periods, String);
                this.dayTypes = SJX.fromArrayOfValues(raw.daytypes, String);
            },

            chlk.models.id.DepartmentId, 'departmentId',
            String, 'description',
            chlk.models.id.ClassId, 'id',
            ArrayOf(chlk.models.id.MarkingPeriodId), 'markingPeriodsId',
            ArrayOf(chlk.models.id.SchoolPersonId), 'teachersIds',
            String, 'classNumber',
            String, 'name',
            READONLY, String, 'fullClassName',
            chlk.models.id.SchoolId, 'schoolId',
            chlk.models.id.CourseTypeId, 'courseTypeId',
            chlk.models.classes.Room, 'room',
            ArrayOf(String), 'periods',
            ArrayOf(String), 'dayTypes',

            String, function getFullClassName(){
                var classNumber = this.getClassNumber();
                if(classNumber) return classNumber + " " + this.getName();
                return this.getName();
            },

            chlk.models.people.User, 'teacher',

            Number, 'defaultAnnouncementTypeId'
        ]);
});
