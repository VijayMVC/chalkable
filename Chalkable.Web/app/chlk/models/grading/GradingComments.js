REQUIRE('chlk.models.grading.GradingComment');
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
            },

            [[ArrayOf(chlk.models.grading.GradingComment)]],
            function $createFromList(comments_){
                BASE();
                var items = (comments_ || []).map(function(item){
                    return item.getComment();
                });
                this.setComments(items);
            }

        ]);
});
