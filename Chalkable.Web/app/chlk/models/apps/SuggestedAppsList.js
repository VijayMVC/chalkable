REQUIRE('chlk.models.apps.ApplicationForAttach');
REQUIRE('chlk.models.id.ClassId');
REQUIRE('chlk.models.id.AnnouncementId');

NAMESPACE('chlk.models.apps', function() {
    "use strict";

    /** @class chlk.models.apps.SuggestedAppsList*/

    CLASS(
        'SuggestedAppsList', [

            ArrayOf(chlk.models.apps.ApplicationForAttach), 'suggestedApps',
            chlk.models.id.ClassId, 'classId',
            chlk.models.id.AnnouncementId, 'announcementId',

            [[chlk.models.id.ClassId, chlk.models.id.AnnouncementId, ArrayOf(chlk.models.apps.ApplicationForAttach)]],
            function $(classId, announcementId, suggestedApps){
                BASE();
                this.setClassId(classId);
                this.setAnnouncementId(announcementId);
                this.setSuggestedApps(suggestedApps);
            }
    ]);

});
