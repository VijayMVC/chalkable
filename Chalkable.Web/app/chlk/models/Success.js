NAMESPACE('chlk.models', function () {
    "use strict";
    /** @class chlk.models.Success*/
    CLASS(
        'Success', [
            Boolean, 'data',

            [[Boolean]],
            function $(success_){
                BASE();
                if(success_ !== undefined)
                    this.setData(success_);
            }
        ]);
});
