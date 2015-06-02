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

            function isSchoolChecked(school){
                return this.getGroupMembers().filter(function(item){return item.getSchoolYearId() == school.getId()}).length > 0
            },

            function isGradeLevelChecked(gradeLevel){
                return this.getGroupMembers().filter(function(item){return item.getGradeLevelId().valueOf() == gradeLevel.getId()}).length > 0
            }
        ])
});
