REQUIRE('chlk.models.common.PaginatedList');
REQUIRE('chlk.models.common.BaseAttachViewData');

NAMESPACE('chlk.models.apps', function () {

    "use strict";
    /** @class chlk.models.apps.AppsForAttachViewData*/
    CLASS(
        'AppsForAttachViewData', EXTENDS(chlk.models.common.BaseAttachViewData), [

            chlk.models.common.PaginatedList, 'apps',

            Number, 'start',

            [[chlk.models.common.AttachOptionsViewData, chlk.models.common.PaginatedList, Number]],
            function $(options, apps, start_){
                BASE(options);
                this.setApps(apps);
                this.setStart(start_ || 0);
            }
        ]);
});
