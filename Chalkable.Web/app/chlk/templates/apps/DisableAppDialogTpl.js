REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.apps.ApplicationBanViewData');

NAMESPACE('chlk.templates.apps', function(){

    /**@class chlk.templates.apps.DisableAppDialogTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/apps/DisableAppDialog.jade')],
        [ria.templates.ModelBind(chlk.models.apps.ApplicationBanViewData)],
        'DisableAppDialogTpl', EXTENDS(chlk.templates.ChlkTemplate), [

            [ria.templates.ModelPropertyBind],
            chlk.models.id.AppId, 'applicationId',

            [ria.templates.ModelPropertyBind],
            String, 'requestId',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.apps.ApplicationSchoolBan), 'schools',

            ArrayOf(chlk.models.id.SchoolId), function getBannedSchoolIds(){
                return this.getSchools()
                    .filter(function(_){ return _.isBanned();})
                    .map(function(_){ return _.getSchoolId();});
            },

            Number, function getAllSchoolsStage(){
                var bannedSchoolIds = this.getBannedSchoolIds();
                var schools = this.getSchools();
                return bannedSchoolIds && bannedSchoolIds.length > 0 ? (schools.length <= bannedSchoolIds.length ? 2 : 1 ) : 0;
            },

            String, function getBannedSchoolIdsStr(){
                return this.getBannedSchoolIds().map(function(_){return _.valueOf()}).join(',');
            }
        ]);
});
