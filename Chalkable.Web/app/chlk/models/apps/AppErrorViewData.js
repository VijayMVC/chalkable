NAMESPACE('chlk.models.apps', function () {
    "use strict";

    /** @class chlk.models.apps.AppErrorViewData*/
    CLASS(
        'AppErrorViewData', [
            String, 'url',

            [[String]],
            function $(url){
                BASE();
                this.setUrl(url);
            }
        ]);
});
