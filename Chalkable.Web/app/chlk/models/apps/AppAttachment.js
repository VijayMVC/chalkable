REQUIRE('chlk.models.id.AnnouncementId');
REQUIRE('chlk.models.id.AnnouncementApplicationId');
REQUIRE('chlk.models.id.SchoolPersonId');
REQUIRE('chlk.models.apps.Application');

NAMESPACE('chlk.models.apps', function () {
    "use strict";
    /** @class chlk.models.apps.AppAttachment*/
    CLASS(
        'AppAttachment', EXTENDS(chlk.models.apps.Application), [
            [ria.serialize.SerializeProperty('announcementapplicationid')],
            chlk.models.id.AnnouncementApplicationId, 'announcementApplicationId',
            [ria.serialize.SerializeProperty('announcementid')],
            chlk.models.id.AnnouncementId, 'announcementId',
            [ria.serialize.SerializeProperty('currentpersonid')],
            chlk.models.id.SchoolPersonId, 'currentPersonId',
            Boolean, 'active',
            [ria.serialize.SerializeProperty('viewurl')],
            String, 'viewUrl',
            [ria.serialize.SerializeProperty('editurl')],
            String, 'editUrl',
            [ria.serialize.SerializeProperty('gradingviewurl')],
            String, 'gradingViewUrl',
            Number, 'order',
            [ria.serialize.SerializeProperty('installedforme')],
            Boolean, 'installedForMe',
            String, 'oauthCode',
            String, 'currentModeUrl'
    ])

});
