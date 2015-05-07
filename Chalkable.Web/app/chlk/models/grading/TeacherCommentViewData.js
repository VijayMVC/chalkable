REQUIRE('chlk.models.id.TeacherCommentId');
REQUIRE('chlk.models.id.SchoolPersonId');

NAMESPACE('chlk.models.grading', function () {
    "use strict";

    /** @class chlk.models.grading.TeacherCommentViewData*/
    CLASS('TeacherCommentViewData', [
        [ria.serialize.SerializeProperty('commentid')],
        chlk.models.id.TeacherCommentId, 'commentId',

        [ria.serialize.SerializeProperty('teacherid')],
        chlk.models.id.SchoolPersonId, 'teacherId',

        [ria.serialize.SerializeProperty('editableforteacher')],
        Boolean, 'editable',

        [ria.serialize.SerializeProperty('issystem')],
        Boolean, 'system',

        String, 'comment',

        String, 'ids',

        Boolean, 'ableEdit'

    ]);
});
