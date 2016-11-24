REQUIRE('chlk.models.student.StudentInfo');
REQUIRE('chlk.models.panorama.StudentPanoramaViewData');

NAMESPACE('chlk.models.student', function(){
    "use strict";

    var SJX = ria.serialize.SJX;

    /**@class chlk.models.student.StudentPanoramaViewData*/

    CLASS('StudentPanoramaViewData', EXTENDS(chlk.models.student.StudentInfo),[

        chlk.models.panorama.StudentPanoramaViewData, 'panoramaInfo'
    ]);
});