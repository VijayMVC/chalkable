REQUIRE('ria.serialize.SJX');
REQUIRE('ria.serialize.IDeserializable');
REQUIRE('chlk.models.id.AnnouncementId');
REQUIRE('chlk.models.id.AnnouncementApplicationId');
REQUIRE('chlk.models.id.SchoolPersonId');
REQUIRE('chlk.models.apps.AppMarketApplication');

NAMESPACE('chlk.models.apps', function () {
    "use strict";

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.apps.AppAttachment*/
    CLASS(
        UNSAFE, 'AppAttachment', EXTENDS(chlk.models.apps.AppMarketApplication), IMPLEMENTS(ria.serialize.IDeserializable), [
            OVERRIDE, VOID, function deserialize(raw){
                BASE(raw);
                this.announcementApplicationId = SJX.fromValue(raw.announcementapplicationid, chlk.models.id.AnnouncementApplicationId);
                this.announcementId = SJX.fromValue(raw.announcementid, chlk.models.id.AnnouncementId);
                this.currentPersonId = SJX.fromValue(raw.currentpersonid, chlk.models.id.SchoolPersonId);
                this.active = SJX.fromValue(raw.active, Boolean);
                this.viewUrl = SJX.fromValue(raw.viewurl, String);
                this.editUrl = SJX.fromValue(raw.editurl, String);
                this.gradingViewUrl = SJX.fromValue(raw.gradingviewurl, String);
                this.order = SJX.fromValue(raw.order, Number);
                this.installedForMe = SJX.fromValue(raw.installedforme, Boolean);
                this.oauthCode = SJX.fromValue(raw.oauthcode, String);
                this.currentModeUrl = SJX.fromValue(raw.currentmodeurl, String);
            },
            chlk.models.id.AnnouncementApplicationId, 'announcementApplicationId',
            chlk.models.id.AnnouncementId, 'announcementId',
            chlk.models.id.SchoolPersonId, 'currentPersonId',
            Boolean, 'active',
            String, 'viewUrl',
            String, 'editUrl',
            String, 'gradingViewUrl',
            Number, 'order',
            Boolean, 'installedForMe',
            String, 'oauthCode',
            String, 'currentModeUrl',

            [[String, String, chlk.models.id.AnnouncementApplicationId, chlk.models.apps.AppMarketApplication]],
            function $create(currentModeUrl, oauthCode, announcementAppId_, appData_){
                BASE();
                this.setCurrentModeUrl(currentModeUrl);
                this.setOauthCode(oauthCode);
                this.setAnnouncementApplicationId(announcementAppId_ || new chlk.models.id.AnnouncementApplicationId(''));
                if (!this.getAnnouncementId()){
                    this.setAnnouncementId(new chlk.models.id.AnnouncementId(''));
                }

                if (appData_){
                    this.setPersonal(appData_.isPersonal());
                    this.setAppAccess(appData_.getAppAccess());
                }
            }
    ])

});
