REQUIRE('ria.serialize.SJX');
REQUIRE('ria.serialize.IDeserializable');
REQUIRE('chlk.models.apps.Application');
REQUIRE('chlk.models.developer.DeveloperInfo');
REQUIRE('chlk.models.apps.AppInstallGroup');
REQUIRE('chlk.models.apps.AppInstallInfo');
REQUIRE('chlk.models.apps.AppRating');

REQUIRE('chlk.models.common.Role');
REQUIRE('chlk.models.common.ChlkDate');

REQUIRE('chlk.models.common.NameId');

NAMESPACE('chlk.models.apps', function () {
    "use strict";

    var SJX = ria.serialize.SJX;


    /** @class chlk.models.apps.ApplicationActionEnum */
    ENUM('ApplicationActionEnum',{
        INSTALL:0,
        UNINSTALL:1,
        BAN:2,
        UN_BAN:3
    });

    /** @class chlk.models.apps.ApplicationInstallRecord */
    CLASS(
        UNSAFE, 'ApplicationInstallRecord', IMPLEMENTS(ria.serialize.IDeserializable), [

            VOID, function deserialize(raw) {
                this.personId = SJX.fromValue(raw.personid, chlk.models.id.SchoolPersonId);
                this.firstName = SJX.fromValue(raw.firstname, String);
                this.lastName = SJX.fromValue(raw.lastname, String);
                this.ownerRoleId = SJX.fromValue(raw.ownerroleid, chlk.models.common.RoleEnum);

                this.installedCount = SJX.fromValue(raw.installedcount, Number);
                this.price = SJX.fromValue(raw.price, Number);
                this.remains = SJX.fromValue(raw.remains, Number);

                this.schools = SJX.fromArrayOfDeserializables(raw.schools, chlk.models.common.NameId);

                this.installDate = SJX.fromDeserializable(raw.date, chlk.models.common.ChlkDate);
                this.action = SJX.fromValue(raw.action, chlk.models.apps.ApplicationActionEnum);
            },

            chlk.models.id.SchoolPersonId, 'personId',
            String, 'firstName',
            String, 'lastName',
            chlk.models.common.RoleEnum, 'ownerRoleId',

            Number, 'installedCount',
            Number, 'price',
            Number, 'remains',

            ArrayOf(chlk.models.common.NameId), 'schools',

            chlk.models.common.ChlkDate, 'installDate',

            chlk.models.apps.ApplicationActionEnum, 'action',

            String, function getActionName(){
                switch (this.getAction()){
                    case chlk.models.apps.ApplicationActionEnum.INSTALL:  return 'Installed';
                    case chlk.models.apps.ApplicationActionEnum.UNINSTALL: return 'Uninstalled';
                    case chlk.models.apps.ApplicationActionEnum.BAN: return 'Banned';
                    case chlk.models.apps.ApplicationActionEnum.UN_BAN: return 'UnBanned'
                }
                return null;
            },

            Boolean, function isBanUnBanAction(){
                var action = this.getAction();
                return action == chlk.models.apps.ApplicationActionEnum.BAN || action == chlk.models.apps.ApplicationActionEnum.UN_BAN;
            },

            String, function getFullName() {
                return [this.firstName, this.lastName].filter(function (_) { return _ }).join(' ');
            }
        ]);

    /** @class chlk.models.apps.AppMarketApplication*/
    CLASS(
        UNSAFE,  'AppMarketApplication', EXTENDS(chlk.models.apps.Application), IMPLEMENTS(ria.serialize.IDeserializable), [
            
            OVERRIDE, VOID, function deserialize(raw){
                BASE(raw);
                this.installedForGroups = raw.installedforpersonsgroup
                this.installedOnlyForCurrentUser = SJX.fromValue(raw.isinstalledonlyforme, Boolean);
                this.alreadyInstalled = SJX.fromValue(raw.alreadyinstalled, Boolean);
                this.applicationInstalls = SJX.fromArrayOfDeserializables(raw.applicationinstalls, chlk.models.apps.AppInstallInfo);
                this.uninstallable = SJX.fromValue(raw.uninstallable, Boolean);
                this.selfInstalled = SJX.fromValue(raw.selfinstalled, Boolean);
                this.personal = SJX.fromValue(raw.personal, Boolean);
                this.applicationInstallIds = SJX.fromValue(raw.applicationinstallids, String);
                this.applicationRating = SJX.fromDeserializable(raw.applicationrating, chlk.models.apps.AppRating);
                this.applicationHistory = SJX.fromArrayOfDeserializables(raw.applicationhistory, chlk.models.apps.ApplicationInstallRecord);
            },
            Array, 'installedForGroups',
            Boolean, 'installedOnlyForCurrentUser',
            ArrayOf(chlk.models.apps.AppInstallInfo), 'applicationInstalls',
            Boolean, 'uninstallable',
            Boolean, 'selfInstalled',
            Boolean, 'personal',
            String,  'applicationInstallIds',
            Boolean, 'alreadyInstalled',
            chlk.models.apps.AppRating, 'applicationRating',
            ArrayOf(chlk.models.apps.ApplicationInstallRecord), 'applicationHistory'
        ]);


});
