REQUIRE('chlk.models.people.User');
REQUIRE('chlk.models.id.CourseId');
REQUIRE('chlk.models.id.DepartmentId');

NAMESPACE('chlk.models.course', function () {
    "use strict";

    /** @class chlk.models.course.Course*/
    CLASS(
        'Course', [
            String, 'code',

            [ria.serialize.SerializeProperty('departmentid')],
            chlk.models.id.DepartmentId, 'departmentId',

            chlk.models.id.CourseId, 'id',

            String, 'title'
        ]);
});
