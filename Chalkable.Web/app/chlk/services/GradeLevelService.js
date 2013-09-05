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
            },

            ArrayOf(chlk.models.classes.ClassForTopBar), 'gradesToFilter',
            ArrayOf(chlk.models.classes.ClassForTopBar), 'gradesToFilterWithAll',

            [[Boolean]],
            Array, function getGradeLevelsForTopBar(withAll_) {
                var res = this.getGradesToFilter(), res1 = this.getGradesToFilterWithAll();
                if(res)
                    return withAll_ ? res1 : res;
                res = new ria.serialize.JsonSerializer().deserialize(window.gradeLevels, ArrayOf(chlk.models.classes.ClassForTopBar));
                this.setClassesToFilter(res);
                var gradesToFilterWithAll = window.gradeLevels.slice();
                gradesToFilterWithAll.unshift({
                    name: 'All'
                });
                res1 = new ria.serialize.JsonSerializer().deserialize(gradesToFilterWithAll, ArrayOf(chlk.models.classes.ClassForTopBar));
                this.setClassesToFilterWithAll(res1);
                return withAll_ ? res1 : res;
            }
        ])
});