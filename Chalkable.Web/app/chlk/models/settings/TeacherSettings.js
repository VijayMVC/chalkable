REQUIRE('chlk.models.settings.SchoolPersonSettings');

NAMESPACE('chlk.models.settings', function () {
    "use strict";
    /** @class chlk.models.settings.TeacherSettings*/
    CLASS('TeacherSettings', EXTENDS(chlk.models.settings.SchoolPersonSettings),[
        [[chlk.models.id.SchoolPersonId]],
        function $(personId_){
            BASE(personId_);
        }
    ]);
});
