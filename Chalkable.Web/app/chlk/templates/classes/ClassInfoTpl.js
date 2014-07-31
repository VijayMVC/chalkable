REQUIRE('chlk.templates.profile.ClassProfileTpl');
REQUIRE('chlk.models.classes.ClassProfileInfoViewData');

NAMESPACE('chlk.templates.classes', function () {

    /** @class chlk.templates.classes.ClassInfoTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/class/ClassInfo.jade')],
        [ria.templates.ModelBind(chlk.models.classes.ClassProfileInfoViewData)],
        'ClassInfoTpl', EXTENDS(chlk.templates.profile.ClassProfileTpl), [

            chlk.models.classes.ClassInfo, function getClassInfo(){return this.getClazz();},

            Object, function prepareDetailedInfo(){
                var model = this.getClassInfo();
                var res = [
                    {title: 'TEACHER', value: model.getTeacher().getDisplayName()}
                ];
                if(model.getRoom())
                    res.push({title: 'ROOM NUMBER', value: model.getRoom().getRoomNumber() || ''});
                if(model.getDepartment())
                    res.push({title: 'DEPARTMENT', value: model.getDepartment().getName()});
                res.push({title: 'GRADE', value: model.getGradeLevel().getFullText() || ''});
                return res;
            }
        ])
});