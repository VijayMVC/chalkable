REQUIRE('chlk.models.settings.AddCourseToPanoramaViewData');
REQUIRE('chlk.templates.ChlkTemplate');

NAMESPACE('chlk.templates.settings', function () {
    "use strict";
    /** @class chlk.templates.settings.AddCourseToPanoramaTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/settings/AddCourseToPanorama.jade')],
        [ria.templates.ModelBind(chlk.models.settings.AddCourseToPanoramaViewData)],
        'AddCourseToPanoramaTpl', EXTENDS(chlk.templates.ChlkTemplate), [

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.classes.CourseType), 'courseTypes',

            [ria.templates.ModelPropertyBind],
            String, 'requestId',

            [ria.templates.ModelPropertyBind],
            Array, 'excludeIds'
        ])
});