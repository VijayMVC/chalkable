REQUIRE('chlk.models.classes.CourseType');

NAMESPACE('chlk.models.settings', function () {
    "use strict";

    /** @class chlk.models.settings.AddCourseToPanoramaViewData*/
    CLASS('AddCourseToPanoramaViewData', [

        ArrayOf(chlk.models.classes.CourseType), 'courseTypes',
        String, 'requestId',
        Array, 'excludeIds',

        function $(courseTypes_, excludeIds_, requestId_){
            BASE();
            courseTypes_ && this.setCourseTypes(courseTypes_);
            excludeIds_ && this.setExcludeIds(excludeIds_);
            requestId_ && this.setRequestId(requestId_);
        }
    ]);
});
