REQUIRE('ria.serialize.SJX');
REQUIRE('chlk.models.classes.CourseType');
REQUIRE('chlk.models.profile.StandardizedTestFilterViewData');

NAMESPACE('chlk.models.panorama', function () {
    "use strict";

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.panorama.CourseTypeSettingViewData*/
    CLASS(
        UNSAFE, 'CourseTypeSettingViewData', IMPLEMENTS(ria.serialize.IDeserializable), [
            chlk.models.classes.CourseType, 'courseType',
            ArrayOf(chlk.models.profile.StandardizedTestFilterViewData), 'standardizedTestFilters',

            VOID, function deserialize(raw) {
                this.courseType = SJX.fromDeserializable(raw.coursetype, chlk.models.classes.CourseType);
                this.standardizedTestFilters = SJX.fromArrayOfDeserializables(raw.standardizedtestfilters, chlk.models.profile.StandardizedTestFilterViewData);
            },

            [[chlk.models.classes.CourseType]],
            function $(courseType_){
                BASE();
                if(courseType_)
                    this.setCourseType(courseType_);
            }
        ]);
});
