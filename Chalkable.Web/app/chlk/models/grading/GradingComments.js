NAMESPACE('chlk.models.grading', function () {
    "use strict";
    /** @class chlk.models.grading.GradingComments*/
    CLASS(
        'GradingComments', [
            ArrayOf(String), 'comments',

            [[ArrayOf(String)]],
            function $(comments_){
                BASE();
                if(comments_)
                    this.setComments(comments_);
            }
        ]);
});
