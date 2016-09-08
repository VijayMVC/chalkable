REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.settings.ReportCardsSettingsViewData');

NAMESPACE('chlk.templates.settings', function () {
    "use strict";
    /** @class chlk.templates.settings.ReportCardsSettingsTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/settings/ReportCardsSettings.jade')],
        [ria.templates.ModelBind(chlk.models.settings.ReportCardsSettingsViewData)],
        'ReportCardsSettingsTpl', EXTENDS(chlk.templates.ChlkTemplate), [

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.settings.ReportCardsLogo), 'listOfLogo',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.apps.Application), 'applications',

            Boolean, 'ableToUpdate',

            Boolean, function hasDistrictLogo(){
                var listOfLogo = this.getListOfLogo();
                return listOfLogo.filter(function(logo){return logo.isDistrictLogo();}).count > 0;
            },

            ArrayOf(chlk.models.settings.ReportCardsLogo), function schoolsLogo(){
                return  this.getListOfLogo().filter(function(logo){return !logo.isDistrictLogo();});
            },

            chlk.models.settings.ReportCardsLogo, function districtLogo(){
                var res = this.getListOfLogo().filter(function(logo){return logo.isDistrictLogo();});
                return res.length > 0 ? res[0] : null;
            },

            Boolean, function isAdminPanoramaEnabled() {
                return this.getCurrentUser().getClaims().filter(function (item) {
                        return item.hasPermission(chlk.models.people.UserPermissionEnum.MAINTAIN_CHALKABLE_DISTRICT_SETTINGS);
                    }).length > 0;
            },

            function getRandomParam_(){
                return Math.random().toString(36).substr(2) + (new Date).getTime().toString(36);
            },

            ArrayOf(Object), function prepareImages(){
                var res = [], that = this;
                var districtLogo = this.districtLogo();

                if(districtLogo){
                    res.push({
                        name: 'District',
                        address: districtLogo.getLogoAddress() ? districtLogo.getLogoAddress() + '?_=' + this.getRandomParam_() : ''
                    });
                }
                res = res.concat(this.schoolsLogo().map(function(x){
                    return {name: x.getSchoolName(), address: x.getLogoAddress() ? x.getLogoAddress() + '?_=' + that.getRandomParam_() : ''};
                }));

                return res;
            }

        ])
});