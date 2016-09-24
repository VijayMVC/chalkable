REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.recipients.GroupSelectorViewData');

NAMESPACE('chlk.templates.controls.group_people_selector', function () {

    /** @class chlk.templates.controls.group_people_selector.SelectorBaseTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/controls/group-people-selector/selected-items.jade')],
        [ria.templates.ModelBind(chlk.models.recipients.GroupSelectorViewData)],
        'SelectorBaseTpl', EXTENDS(chlk.templates.ChlkTemplate), [
            [ria.templates.ModelPropertyBind],
            chlk.models.recipients.GroupsListViewData, 'groupsPart',

            [ria.templates.ModelPropertyBind],
            chlk.models.recipients.UsersListViewData, 'myStudentsPart',

            [ria.templates.ModelPropertyBind],
            chlk.models.recipients.UsersListViewData, 'allStudentsPart',

            [ria.templates.ModelPropertyBind],
            Boolean, 'hasAccessToAllStudents',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.group.Group), 'selectedGroups',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.people.ShortUserInfo), 'selectedStudents',

            [ria.templates.ModelPropertyBind],
            chlk.models.group.Group, 'groupInfo',

            [ria.templates.ModelPropertyBind],
            Boolean, 'hasOwnStudents',

            [ria.templates.ModelPropertyBind],
            chlk.models.recipients.SelectorModeEnum, 'selectorMode',

            [ria.templates.ModelPropertyBind],
            String, 'requestId',

            [ria.templates.ModelPropertyBind],
            Boolean, 'hasAccessToLE',
            
            function isSubmitDisabled(){
                return this.isTabSelected(4) && !(this.getSelectedGroups() || []).length && !(this.getSelectedStudents() || []).length
            },

            function isTabSelected(index){
                var mode = this.getSelectorMode();
                switch (index){
                    case 1: return mode == chlk.models.recipients.SelectorModeEnum.SELECT_WITH_GROUPS ||
                        mode == chlk.models.recipients.SelectorModeEnum.VIEW_WITH_GROUPS;
                    case 2: return this.isTabSelected(1) && !this.isHasOwnStudents();
                    case 3: return false;
                    case 4: return !this.isTabSelected(1);
                }
            },
            
            function getGroupIds(){
                return (this.getSelectedGroups() || []).map(function(item){return item.getId().valueOf()})
            },

            function getStudentIds(){
                return (this.getSelectedStudents() || []).map(function(item){return item.getId().valueOf()})
            },

            function getSelectedItemsObj(){
                var groups = [], students = [];
                (this.getSelectedGroups() || []).forEach(function(group){
                    groups.push({id: group.getId().valueOf(), name: group.getName()})
                });
                (this.getSelectedStudents() || []).forEach(function(student){
                    students.push({id: student.getId().valueOf(), displayname: student.getDisplayName(), gender: student.getGender()})
                });
                return JSON.stringify({groups: groups, students: students});
            },

            function getPageTitle(){
                var mode = this.getSelectorMode();
                switch (mode){
                    case chlk.models.recipients.SelectorModeEnum.SELECT_WITH_GROUPS: return "Select Recipients";
                    case chlk.models.recipients.SelectorModeEnum.SELECT_WITHOUT_GROUPS: return "Create Group";
                    case chlk.models.recipients.SelectorModeEnum.EDIT_WITHOUT_GROUPS: return "Edit Group - " + (this.getGroupInfo() && this.getGroupInfo().getName());
                    case chlk.models.recipients.SelectorModeEnum.VIEW_WITH_GROUPS: return "People";
                }
            }
        ])
});