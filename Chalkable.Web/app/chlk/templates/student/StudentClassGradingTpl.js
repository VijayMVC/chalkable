REQUIRE('chlk.templates.JadeTemplate');
REQUIRE('chlk.models.student.ClassPersonGradingInfo');

NAMESPACE('chlk.templates.student', function(){
    "use strict";

    /**@class chlk.templates.student.StudentClassGradingTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/student/StudentClassGradingItemView.jade')],
        [ria.templates.ModelBind(chlk.models.student.ClassPersonGradingInfo)],
        'StudentClassGradingTpl', EXTENDS(chlk.templates.JadeTemplate),[

            [ria.templates.ModelPropertyBind],
            chlk.models.id.ClassPersonId, 'classPersonId',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.ClassId, 'classId',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.SchoolPersonId, 'personId',

            [ria.templates.ModelPropertyBind],
            String, 'className',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.CourseId, 'courseId',

            [ria.templates.ModelPropertyBind],
            Number, 'classAvg',

            [ria.templates.ModelPropertyBind],
            Number, 'studentAvg',

            Object, function prepareGlanceBoxData(){
                console.log('test')
                return {
                    value: this.getClassAvg() || 0,
                    title: this.getClassName()
                }
            }
    ]);
});