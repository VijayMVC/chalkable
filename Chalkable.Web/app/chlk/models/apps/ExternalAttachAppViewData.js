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
            chlk.models.id.AnnouncementApplicationId, 'announcementApplicationId',

            [[chlk.models.common.AttachOptionsViewData, chlk.models.apps.Application, String, String, chlk.models.id.AnnouncementApplicationId]],
            function $(options, app, url, title, announcementApplicationId_){
                BASE(options);

                this.setApp(app);
                this.setUrl(url);
                this.setTitle(title);
                announcementApplicationId_ && this.setAnnouncementApplicationId(announcementApplicationId_);
            }
        ]);
});
