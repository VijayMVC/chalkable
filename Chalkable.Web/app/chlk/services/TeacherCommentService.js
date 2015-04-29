REQUIRE('chlk.services.BaseService');
REQUIRE('ria.async.Future');
REQUIRE('chlk.models.grading.TeacherCommentViewData');

NAMESPACE('chlk.services', function () {
    "use strict";

    /** @class chlk.services.TeacherCommentService */
    CLASS(
        'TeacherCommentService', EXTENDS(chlk.services.BaseService), [
            [[chlk.models.id.SchoolPersonId]],
            ria.async.Future, function getTeacherComments(teacherId) {
                return this.get('TeacherComment/CommentsList.json', ArrayOf(chlk.models.grading.TeacherCommentViewData), {
                    teacherId : teacherId.valueOf()
                });
            },

            [[chlk.models.id.TeacherCommentId, String]],
            ria.async.Future, function updateComment(commentId, comment) {
                return this.get('TeacherComment/Update.json', null, {
                    commentId : commentId.valueOf(),
                    comment : comment
                });
            },

            [[chlk.models.id.TeacherCommentId]],
            ria.async.Future, function createComment(comment) {
                return this.get('TeacherComment/Create.json', null, {
                    comment : comment
                });
            },
            
            ria.async.Future, function deleteComments(commentIds) {
                return this.get('TeacherComment/DeleteComments.json', null, {
                    commentIds : this.arrayToCsv(commentIds)
                });
            }
        ])
});