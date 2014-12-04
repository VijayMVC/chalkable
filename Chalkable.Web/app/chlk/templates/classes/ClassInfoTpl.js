REQUIRE('chlk.templates.profile.ClassProfileTpl');
REQUIRE('chlk.models.classes.ClassProfileInfoViewData');

NAMESPACE('chlk.templates.classes', function () {

    /** @class chlk.templates.classes.ClassInfoTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/class/ClassInfo.jade')],
        [ria.templates.ModelBind(chlk.models.classes.ClassProfileInfoViewData)],
        'ClassInfoTpl', EXTENDS(chlk.templates.profile.ClassProfileTpl), [

            chlk.models.classes.ClassInfo, function getClassInfo(){
                return this.getClazz();
            },

            Object, function prepareDetailedInfo(){
                var model = this.getClassInfo();
                var teacher = model.getTeacher();
                var room = model.getRoom();
                var department = model.getDepartment();
                var res = [];
                if (teacher)
                    res.push({title: 'TEACHER', value: teacher.getDisplayName()});
                if(room)
                    res.push({title: 'ROOM NUMBER', value: room.getRoomNumber() || ''});
                if(department)
                    res.push({title: 'DEPARTMENT', value: department.getName()});
                res.push({title: 'GRADE', value: model.getGradeLevel().getName() || ''});
                return res;
            }
        ])
});