REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.group.GroupExplorer');

NAMESPACE('chlk.templates.group', function () {

    /** @class chlk.templates.group.GroupExplorerTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/announcement/AnnouncementGroupGradeLevels.jade')],
        [ria.templates.ModelBind(chlk.models.group.GroupExplorer)],
        'GroupExplorerTpl', EXTENDS(chlk.templates.ChlkTemplate), [
            [ria.templates.ModelPropertyBind],
            chlk.models.group.Group, 'group',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.common.NameId), 'gradeLevels',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.school.School), 'schools',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.group.GroupMember), 'groupMembers',

            function getAllSchoolsStage(){
                var len = 0, len2 = 0, stage;
                this.getSchools().forEach(function(school){
                    stage = this.getSchoolStage(school);
                    if(stage == 2)
                        len++;
                    if(stage == 1)
                        len2++;
                }, this);

                if(!len && !len2)
                    return 0;

                if(len2 || len < this.getSchools().length)
                    return 1;

                return 2;
            },

            function getSchoolStage(school){
                var len = this.getGroupMembers().filter(function(item){return item.getSchoolYearId() == school.getSchoolYearId()}).length ;
                var len1 = this.getGroupMembers().filter(function(item){return item.getSchoolYearId() == school.getSchoolYearId() && item.getMemberState() == 1}).length ;
                var len2 = this.getGroupMembers().filter(function(item){return item.getSchoolYearId() == school.getSchoolYearId() && item.getMemberState() == 2}).length ;

                if(!len1 && !len2)
                    return 0;

                if(len1 || len2 < len)
                    return 1;

                return 2;
            },

            function getGradeLevelStage(gradeLevel, school){
                var item = this.getGroupMembers().filter(function(item){return item.getGradeLevelId().valueOf() == gradeLevel.getId() && item.getSchoolYearId() == school.getSchoolYearId()})[0];

                if(!item)
                    return -1;

                return item.getMemberState();
            }
        ])
});
