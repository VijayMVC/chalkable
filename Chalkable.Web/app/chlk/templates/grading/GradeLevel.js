REQUIRE('chlk.templates.JadeTemplate');
REQUIRE('chlk.models.grading.GradeLevel');

NAMESPACE('chlk.templates.grading', function () {
    "use strict";
    /** @class chlk.templates.grading.GradeLevel*/
    CLASS(
        [ria.templates.ModelBind(chlk.models.grading.GradeLevel)],
        'GradeLevel', EXTENDS(chlk.templates.JadeTemplate), [
            [ria.templates.ModelPropertyBind],
            chlk.models.id.GradeLevelId, 'id',

            [ria.templates.ModelPropertyBind],
            String, 'name',

            [ria.templates.ModelPropertyBind],
            Number, 'number'
        ]);
});
