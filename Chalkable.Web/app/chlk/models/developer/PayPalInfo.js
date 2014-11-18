NAMESPACE('chlk.models.developer', function () {
    "use strict";
    /** @class chlk.models.developer.PayPalInfo*/
    CLASS(
        'PayPalInfo', [
            String, 'email',

            String, 'confirmation',

            [[String]],
            function $(email_){
                BASE();
                if(email_)
                    this.setEmail(email_);
            }
        ]);
});
