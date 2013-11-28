REQUIRE('chlk.models.apps.AppMarketDetailsViewData');
REQUIRE('chlk.templates.apps.AppMarketBaseTpl');

NAMESPACE('chlk.templates.apps', function () {

    /** @class chlk.templates.apps.AppMarketDetails*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/apps/app-market-details.jade')],
        [ria.templates.ModelBind(chlk.models.apps.AppMarketDetailsViewData)],
        'AppMarketDetails', EXTENDS(chlk.templates.apps.AppMarketBaseTpl), [

            [ria.templates.ModelPropertyBind],
            chlk.models.apps.AppMarketApplication, 'app',

            [ria.templates.ModelPropertyBind],
            String, 'installBtnTitle',

            [ria.templates.ModelPropertyBind],
            Boolean, 'installed',


            [[Number]],
            function getRoleTypeClass(typeId){
                var result = "permission-type-none";

                switch(typeId){
                    case chlk.models.common.RoleEnum.STUDENT.valueOf(): result = "permission-role-student";break;
                    case chlk.models.common.RoleEnum.TEACHER.valueOf(): result = "permission-role-teacher";break;
                }

                return result;
            },


            [[Number]],
            function getPermissionTypeClass(typeId){

                var result = "permission-type-none";

                switch(typeId){
                    case chlk.models.apps.AppPermissionTypeEnum.GRADE.valueOf(): result = "permission-grade";break;
                    case chlk.models.apps.AppPermissionTypeEnum.ATTENDANCE.valueOf(): result = "permission-attendance";break;
                    case chlk.models.apps.AppPermissionTypeEnum.SCHEDULE.valueOf(): result = "permission-schedule";break;
                    case chlk.models.apps.AppPermissionTypeEnum.DISCIPLINE.valueOf(): result = "permission-discipline";break;
                    case chlk.models.apps.AppPermissionTypeEnum.MESSAGE.valueOf(): result = "permission-message";break;
                }

                return result;
            },

            [[Number]],
            function getPlatformTypeClass(typeId){
                var result = 'platform-type-none';
                switch (typeId){
                    case chlk.models.apps.AppPlatformTypeEnum.WEB.valueOf(): result = "platform-web";break;
                    case chlk.models.apps.AppPlatformTypeEnum.IOS.valueOf(): result = "platform-ios";break;
                    case chlk.models.apps.AppPlatformTypeEnum.ANDROID.valueOf(): result = "platform-android";break;
                }
                return result;
            },

            [[String]],
            function getVideoUrl(url){
                var res = url;
                if(url.indexOf('vimeo') > -1){
                    res = 'https://player.vimeo.com/video/' + (url && url.slice(url.lastIndexOf('/') + 1));
                }
                if(url.indexOf('youtube') > -1){
                    res = 'https://www.youtube.com/v/' + (url && url.slice(url.lastIndexOf('v=') + 2));
                }
                return res;
            }
        ])
});