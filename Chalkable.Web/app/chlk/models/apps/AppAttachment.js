REQUIRE('chlk.models.id.AnnouncementId');
REQUIRE('chlk.models.id.AnnouncementApplicationId');
REQUIRE('chlk.models.id.SchoolPersonId');

NAMESPACE('chlk.models.apps', function () {
    "use strict";
    /** @class chlk.models.apps.AppAttachment*/
    CLASS(
        'AppAttachment', [
            chlk.models.id.AnnouncementApplicationId, 'announcementApplicationId',
            chlk.models.id.AnnouncementId, 'announcementId',
            chlk.models.id.SchoolPersonId, 'currentPersonId',
            Boolean, 'active',
            String, 'viewUrl',
            String, 'editUrl',
            String, 'gradingViewUrl',
            Number, 'order',
            Boolean, 'installedForMe'
    ])

});
