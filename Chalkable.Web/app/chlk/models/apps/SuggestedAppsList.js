REQUIRE('chlk.models.apps.Application');
REQUIRE('chlk.models.id.ClassId');
REQUIRE('chlk.models.id.AnnouncementId');

NAMESPACE('chlk.models.apps', function() {
    "use strict";

    /** @class chlk.models.apps.SuggestedAppsList*/

    CLASS(
        'SuggestedAppsList', [

            ArrayOf(chlk.models.standard.Standard), 'standards',
            ArrayOf(chlk.models.apps.Application), 'suggestedApps',
            chlk.models.id.ClassId, 'classId',
            chlk.models.id.AnnouncementId, 'announcementId',
            String, 'standardUrlComponents',
            chlk.models.announcement.AnnouncementTypeEnum, 'announcementType',

            [[chlk.models.id.ClassId, chlk.models.id.AnnouncementId, ArrayOf(chlk.models.apps.Application),
                ArrayOf(chlk.models.standard.Standard), String, chlk.models.announcement.AnnouncementTypeEnum]],
            function $(classId_, announcementId_, suggestedApps_, standards_, standardUrlComponents_, announcementType_){
                BASE();
                if(classId_)
                    this.setClassId(classId_);
                if(announcementId_)
                    this.setAnnouncementId(announcementId_);
                if(suggestedApps_)
                    this.setSuggestedApps(suggestedApps_);
                if(standards_)
                    this.setStandards(standards_);
                if(standardUrlComponents_)
                    this.setStandardUrlComponents(standardUrlComponents_);
                if(announcementType_)
                    this.setAnnouncementType(announcementType_);
            }
    ]);

});
