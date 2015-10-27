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
                return this.post('TeacherComment/Update.json', null, {
                    commentId : commentId.valueOf(),
                    comment : comment
                });
            },

            [[String]],
            ria.async.Future, function createComment(comment) {
                return this.post('TeacherComment/Create.json', null, {
                    comment : comment
                });
            },

            [[chlk.models.id.TeacherCommentId, String]],
            ria.async.Future, function createOrUpdateComment(commentId_, comment){
                if(commentId_ && commentId_.valueOf())
                    return this.updateComment(
                        commentId_,
                        comment
                    );
                else
                    return this.createComment(comment);
            },

            ria.async.Future, function deleteComments(commentIds) {
                return this.post('TeacherComment/DeleteComments.json', null, {
                    commentIds : this.arrayToCsv(commentIds)
                });
            }
        ])
});