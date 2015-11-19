NAMESPACE('chlk.models.common', function(){
    "use strict";
    /**@class chlk.models.common.ServerErrorModel*/

    CLASS('ServerErrorModel', [

        String, 'message',
        String, 'stackTrace',

        [[String, String]],
        function $(message_, stackTrace_){
            BASE();
            if(message_)
                this.setMessage(message_);
            if(stackTrace_)
                this.setStackTrace(stackTrace_);
        }
    ]);
});