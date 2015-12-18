REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.group.StudentsForGroupViewData');

NAMESPACE('chlk.templates.group', function () {

    /** @class chlk.templates.group.StudentsForGroupTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/announcement/AnnouncementGradeLevelStudents.jade')],
        [ria.templates.ModelBind(chlk.models.group.StudentsForGroupViewData)],
        'StudentsForGroupTpl', EXTENDS(chlk.templates.ChlkTemplate), [
            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.group.StudentForGroup), 'students',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.GroupId, 'groupId',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.GradeLevelId, 'gradeLevelId',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.SchoolYearId, 'schoolYearId',

            function getAllStudentsStage(){
                var len = this.getStudents().filter(function(item){return item.isAssignedToGroup()}).length;

                if(!len)
                    return 0;

                if(len < this.getStudents().length)
                    return 1;

                return 2;
            },

            function getAllStudentIds(){
                return this.getStudents().map(function(item){return item.getId().valueOf().toString()}).join(',')
            }
        ])
});
