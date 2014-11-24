REQUIRE('chlk.models.id.AppId');
REQUIRE('chlk.models.id.PictureId');
REQUIRE('chlk.models.id.SchoolPersonId');
REQUIRE('chlk.models.id.AppGradeLevelId');

REQUIRE('chlk.models.apps.AppPrice');
REQUIRE('chlk.models.apps.AppAccess');
REQUIRE('chlk.models.apps.AppPermission');
REQUIRE('chlk.models.apps.AppPlatform');

REQUIRE('chlk.models.apps.AppCategory');
REQUIRE('chlk.models.apps.AppPicture');
REQUIRE('chlk.models.apps.AppState');
REQUIRE('chlk.models.apps.AppScreenshots');
REQUIRE('chlk.models.common.NameId');
REQUIRE('chlk.models.developer.DeveloperInfo');
REQUIRE('chlk.models.apps.BannedAppData');

NAMESPACE('chlk.models.apps', function () {
    "use strict";
    /** @class chlk.models.apps.AppsListViewData */
    CLASS(
        'AppsListViewData', [

            chlk.models.common.PaginatedList, 'applications',
            ArrayOf(chlk.models.developer.DeveloperInfo), 'developers',
            ArrayOf(chlk.models.apps.AppState), 'applicationStates',

            chlk.models.id.SchoolPersonId, 'developerId',
            Number, 'stateId',

            [[chlk.models.common.PaginatedList
                , ArrayOf(chlk.models.developer.DeveloperInfo)
                , ArrayOf(chlk.models.apps.AppState)
                , chlk.models.id.SchoolPersonId
                , Number
            ]],
            function $(applications_, developers_, appStates_, developerId_, stateId_){
                BASE();
                if(applications_)
                    this.setApplications(applications_);
                if(developers_)
                    this.setDevelopers(developers_);
                if(appStates_)
                    this.setApplicationStates(appStates_);
                if(developerId_)
                    this.setDeveloperId(developerId_);
                if(stateId_)
                    this.setStateId(stateId_);
            }
        ]);


});
