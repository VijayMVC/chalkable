REQUIRE('chlk.models.settings.ReportCardsLogo');
REQUIRE('chlk.models.school.School');
REQUIRE('chlk.models.apps.Application');


NAMESPACE('chlk.models.settings', function (){
    "use strict";

    /**@class chlk.models.settings.ReportCardsSettingsViewData*/
    CLASS('ReportCardsSettingsViewData', [

        ArrayOf(chlk.models.settings.ReportCardsLogo), 'listOfLogo',

        ArrayOf(chlk.models.apps.Application), 'applications',

        Boolean, 'ableToUpdate',

        [[ArrayOf(chlk.models.settings.ReportCardsLogo), ArrayOf(chlk.models.apps.Application), Boolean]],
        function $(listOfLogo_, apps_, ableToUpdate_){
            BASE();
            if(listOfLogo_)
                this.setListOfLogo(listOfLogo_);
            if(apps_)
                this.setApplications(apps_);
            if(ableToUpdate_)
                this.setAbleToUpdate(ableToUpdate_);
        }
    ]);
});