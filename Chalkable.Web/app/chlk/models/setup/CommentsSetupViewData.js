REQUIRE('chlk.models.grading.TeacherCommentViewData');

NAMESPACE('chlk.models.setup', function () {
    "use strict";

    /** @class chlk.models.setup.CommentsSetupViewData*/
    CLASS('CommentsSetupViewData', [

        ArrayOf(chlk.models.grading.TeacherCommentViewData), 'comments',

        Boolean, 'ableEdit',

        [[ArrayOf(chlk.models.grading.TeacherCommentViewData), Boolean]],
        function $(comments_, ableEdit_){
            BASE();
            if(comments_)
                this.setComments(comments_);
            if(ableEdit_)
                this.setAbleEdit(ableEdit_);
        }
    ]);
});
