NAMESPACE('chlk.models.common', function () {
    "use strict";
    /** @class chlk.models.common.RequestResultViewData*/
    CLASS(
        'RequestResultViewData', [
            Boolean, 'success',
            String, 'text',

            [[Boolean, String]],
            function $(result_, text_){
                BASE();
                if(result_ !== undefined)
                    this.setSuccess(result_);
                if(text_)
                    this.setText(text_);
            }
        ]);
});
