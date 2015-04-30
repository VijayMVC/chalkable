REQUIRE('chlk.models.grading.TeacherCommentViewData');

NAMESPACE('chlk.models.setup', function () {
    "use strict";

    /** @class chlk.models.setup.CommentsSetupViewData*/
    CLASS('CommentsSetupViewData', [

        ArrayOf(chlk.models.grading.TeacherCommentViewData), 'comments',

        [[ArrayOf(chlk.models.grading.TeacherCommentViewData)]],
        function $(comments_){
            BASE();
            if(comments_)
                this.setComments(comments_);
        }
    ]);
});
