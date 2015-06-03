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

            function isAllSchoolsChecked(){
                var len = 0;
                this.getSchools().forEach(function(school){
                    if(this.isSchoolChecked(school))
                        len++
                }, this);
                return this.getSchools().length == len;
            },

            function isSchoolChecked(school){
                return this.getGroupMembers().filter(function(item){return item.getSchoolYearId() == school.getSchoolYearId()}).length == this.getGradeLevels().length
            },

            function isGradeLevelChecked(gradeLevel, school){
                return this.getGroupMembers().filter(function(item){return item.getGradeLevelId().valueOf() == gradeLevel.getId() && item.getSchoolYearId() == school.getSchoolYearId()}).length > 0
            }
        ])
});
