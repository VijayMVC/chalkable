REQUIRE('chlk.templates.profile.ClassProfileTpl');
REQUIRE('chlk.models.classes.ClassInfo');

NAMESPACE('chlk.templates.classes', function () {

    /** @class chlk.templates.classes.ClassInfoTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/class/ClassInfo.jade')],
        [ria.templates.ModelBind(chlk.models.classes.ClassInfo)],
        'ClassInfoTpl', EXTENDS(chlk.templates.profile.ClassProfileTpl), [

            [ria.templates.ModelPropertyBind],
            String, 'name',

            [ria.templates.ModelPropertyBind],
            chlk.models.course.Course, 'course',

            [ria.templates.ModelPropertyBind],
            chlk.models.classes.Room, 'room',

            [ria.templates.ModelPropertyBind],
            chlk.models.departments.Department, 'department',

            Object, function prepareDetailedInfo(){
                var model = this.getModel();
                var res = [
                    {title: 'TEACHER', value: model.getTeacher().getDisplayName()}
                ];
                if(this.getRoom())
                    res.push({title: 'ROOM NUMBER', value: this.getRoom().getNumber()});
                if(model.getDepartment())
                    res.push({title: 'DEPARTMENT', value: model.getDepartment().getName()});
                res.push({title: 'GRADE', value: model.getGradeLevel().getFullText()});
                return res;
            }
        ])
});