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
            chlk.models.id.SchoolPersonId, 'personId',

            [[chlk.models.apps.AppMarketApplication,
                chlk.models.apps.AppTotalPrice,
                chlk.models.id.ClassId,
                chlk.models.id.AnnouncementId,
                chlk.models.id.SchoolPersonId]],
            function $(app, totalPrice, classId_, announcementId_, personId_){
                BASE();
                if (totalPrice)
                    this.setAppTotalPrice(totalPrice);
                if(app)
                    this.setApp(app);
                if(classId_)
                    this.setClassId(classId_);
                if(personId_)
                    this.setPersonId(personId_);
                if(announcementId_)
                    this.setAnnouncementId(announcementId_);
            }
        ]);
});
