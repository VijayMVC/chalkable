REQUIRE('chlk.models.id.CourseTypeId');
REQUIRE('chlk.models.classes.CourseType');

NAMESPACE('chlk.models.announcement', function () {
    "use strict";

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.announcement.GroupStudentsFilterViewData*/
    CLASS(
        UNSAFE, 'GroupStudentsFilterViewData', IMPLEMENTS(ria.serialize.IDeserializable), [

            VOID, function deserialize(raw){
                this.groupId = SJX.fromValue(raw.groupId, chlk.models.id.GroupId);
                this.schoolYearId = SJX.fromValue(raw.schoolYearId, chlk.models.id.SchoolYearId);
                this.gradeLevelId = SJX.fromValue(raw.gradeLevelId, chlk.models.id.GradeLevelId);
                this.courseTypes = SJX.fromArrayOfDeserializables(raw.courseTypes, chlk.models.classes.CourseType);
                this.classIds = SJX.fromValue(raw.classIds, String);
            },

            function $(groupId_, schoolYearId_, gradeLevelId_, courseTypes_){
                BASE();
                if(groupId_)
                    this.setGroupId(groupId_);
                if(schoolYearId_)
                    this.setSchoolYearId(schoolYearId_);
                if(gradeLevelId_)
                    this.setGradeLevelId(gradeLevelId_);
                if(courseTypes_)
                    this.setCourseTypes(courseTypes_);
            },

            chlk.models.id.GroupId, 'groupId',
            chlk.models.id.SchoolYearId, 'schoolYearId',
            chlk.models.id.GradeLevelId, 'gradeLevelId',
            ArrayOf(chlk.models.classes.CourseType), 'courseTypes',
            String, 'classIds'
        ]);
});
