NAMESPACE('chlk.models.common', function () {
    "use strict";
    /** @class chlk.models.common.SimpleObject*/
    CLASS(
        'SimpleObject', [
            Object, 'value',

            function $(value_){
                BASE();
                if(value_)
                    this.setValue(value_)
            }
        ]);
});
