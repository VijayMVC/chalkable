REQUIRE('chlk.models.apps.AppMarketApplication');
REQUIRE('chlk.models.apps.AppInstallGroup');
REQUIRE('chlk.models.id.ClassId');
REQUIRE('chlk.models.id.AnnouncementId');

NAMESPACE('chlk.models.apps', function () {
    "use strict";
    /** @class chlk.models.apps.ShortAppInstallViewData*/
    CLASS(
        'ShortAppInstallViewData', [

            chlk.models.apps.AppMarketApplication, 'app',
            chlk.models.id.ClassId, 'classId',
            chlk.models.id.AnnouncementId, 'announcementId',
            chlk.models.apps.AppTotalPrice, 'appTotalPrice',

            [[chlk.models.apps.AppMarketApplication,
                chlk.models.apps.AppTotalPrice,
                chlk.models.id.ClassId,
                chlk.models.id.AnnouncementId]],
            function $(app, totalPrice, classId, announcementId_){
                BASE();
                if (totalPrice)
                    this.setAppTotalPrice(totalPrice);
                if(app)
                    this.setApp(app);
                if(classId)
                    this.setClassId(classId);
                if(announcementId_)
                    this.setAnnouncementId(announcementId_);
            }
        ]);
});
