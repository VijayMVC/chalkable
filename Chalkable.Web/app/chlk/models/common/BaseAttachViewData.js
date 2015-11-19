REQUIRE('chlk.models.common.AttachOptionsViewData');

NAMESPACE('chlk.models.common', function () {

    "use strict";
    /** @class chlk.models.common.BaseAttachViewData*/
    CLASS(
        'BaseAttachViewData', [

            chlk.models.common.AttachOptionsViewData, 'attachOptions',

            [[chlk.models.common.AttachOptionsViewData]],
            function $(options_){
                BASE();
                if(options_)
                    this.setAttachOptions(options_);
            }
        ]);
});
