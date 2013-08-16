REQUIRE('chlk.services.BaseService');
REQUIRE('chlk.models.id.GradeLevelId');
REQUIRE('chlk.models.apps.AppGradeLevel');

NAMESPACE('chlk.services', function () {
    "use strict";

    /** @class chlk.services.GradeLevelService*/
    CLASS(
        'GradeLevelService', EXTENDS(chlk.services.BaseService), [

            ArrayOf(chlk.models.apps.AppGradeLevel), function getGradeLevels() {
                 return [
                     new chlk.models.apps.AppGradeLevel(new chlk.models.id.AppGradeLevelId(1), '1st'),
                     new chlk.models.apps.AppGradeLevel(new chlk.models.id.AppGradeLevelId(2), '2nd'),
                     new chlk.models.apps.AppGradeLevel(new chlk.models.id.AppGradeLevelId(3), '3rd'),
                     new chlk.models.apps.AppGradeLevel(new chlk.models.id.AppGradeLevelId(4), '4th'),
                     new chlk.models.apps.AppGradeLevel(new chlk.models.id.AppGradeLevelId(5), '5th'),
                     new chlk.models.apps.AppGradeLevel(new chlk.models.id.AppGradeLevelId(6), '6th'),
                     new chlk.models.apps.AppGradeLevel(new chlk.models.id.AppGradeLevelId(7), '7th'),
                     new chlk.models.apps.AppGradeLevel(new chlk.models.id.AppGradeLevelId(8), '8th'),
                     new chlk.models.apps.AppGradeLevel(new chlk.models.id.AppGradeLevelId(9), '9th'),
                     new chlk.models.apps.AppGradeLevel(new chlk.models.id.AppGradeLevelId(10), '10th'),
                     new chlk.models.apps.AppGradeLevel(new chlk.models.id.AppGradeLevelId(11), '11th'),
                     new chlk.models.apps.AppGradeLevel(new chlk.models.id.AppGradeLevelId(12), '12th')
                 ].reverse();
            }
        ])
});