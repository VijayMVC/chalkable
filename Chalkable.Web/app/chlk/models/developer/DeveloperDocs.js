
NAMESPACE('chlk.models.developer', function () {
    "use strict";
    /** @class chlk.models.developer.DeveloperDocs*/
    CLASS(
        'DeveloperDocs', [
            String, 'docsUrl',


            [[String]],
            function $(url){
                this.setDocsUrl(url);
            }

        ]);
});
