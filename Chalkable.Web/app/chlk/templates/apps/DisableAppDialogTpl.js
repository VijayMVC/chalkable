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
            ArrayOf(chlk.models.school.School), 'schools',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.id.SchoolId), 'bannedSchoolIds',

            [ria.templates.ModelPropertyBind],
            Boolean, 'banned',

            [[chlk.models.id.SchoolId]],
            Boolean, function isBannedSchool(schoolId){
                var bannedSchoolIds = this.getBannedSchoolIds();
                return bannedSchoolIds && bannedSchoolIds.filter(function(id){return id == schoolId;}).length > 0;
            },

            Number, function getAllSchoolsStage(){
                var bannedSchoolIds = this.getBannedSchoolIds();
                return bannedSchoolIds && bannedSchoolIds.length > 0 ? (this.isBanned() ? 2 : 1 ) : 0;
            },

            String, function getBannedSchoolIdsStr(){
                return this.getBannedSchoolIds().map(function(_){return _.valueOf()}).join(',');
            }
        ]);
});
