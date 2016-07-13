REQUIRE('ria.serialize.SJX');
REQUIRE('chlk.models.panorama.CourseTypeSettingViewData');
REQUIRE('chlk.models.profile.StandardizedTestFilterViewData');

NAMESPACE('chlk.models.panorama', function () {
    "use strict";

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.panorama.AdminPanoramaSettingsViewData*/
    CLASS(
        UNSAFE, 'AdminPanoramaSettingsViewData', IMPLEMENTS(ria.serialize.IDeserializable), [
            ArrayOf(chlk.models.panorama.CourseTypeSettingViewData), 'courseTypeDefaultSettings',
            ArrayOf(chlk.models.profile.StandardizedTestFilterViewData), 'studentDefaultSettings',
            Number, 'previousYearsCount',

            VOID, function deserialize(raw) {
                this.previousYearsCount = SJX.fromValue(raw.previousyearscount, Number);
                this.courseTypeDefaultSettings = SJX.fromArrayOfDeserializables(raw.coursetypedefaultsettings, chlk.models.panorama.CourseTypeSettingViewData);
                this.studentDefaultSettings = SJX.fromArrayOfDeserializables(raw.studentdefaultsettings, chlk.models.profile.StandardizedTestFilterViewData);
            }
        ]);
});
