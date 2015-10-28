REQUIRE('chlk.models.common.PaginatedList');
REQUIRE('chlk.models.common.BaseAttachViewData');

NAMESPACE('chlk.models.apps', function () {

    "use strict";
    /** @class chlk.models.apps.ExternalAttachAppViewData*/
    CLASS(
        'ExternalAttachAppViewData', EXTENDS(chlk.models.common.BaseAttachViewData), [

            chlk.models.apps.Application, 'app',
            String, 'url',
            String, 'title',

            [[chlk.models.common.AttachOptionsViewData, chlk.models.apps.Application, String, String]],
            function $(options, app, url, title){
                BASE(options);

                this.setApp(app);
                this.setUrl(url);
                this.setTitle(title);
            }
        ]);
});
