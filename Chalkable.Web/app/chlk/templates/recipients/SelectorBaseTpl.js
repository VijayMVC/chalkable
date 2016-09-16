REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.recipients.GroupSelectorViewData');

NAMESPACE('chlk.templates.recipients', function () {

    /** @class chlk.templates.recipients.SelectorBaseTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/recipients/SelectorBase.jade')],
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
            String, 'groupName',

            [ria.templates.ModelPropertyBind],
            Boolean, 'hasOwnStudents',

            [ria.templates.ModelPropertyBind],
            chlk.models.recipients.SelectorModeEnum, 'selectorMode',

            function getPageTitle(){
                var mode = this.getSelectorMode();
                switch (mode){
                    case chlk.models.recipients.SelectorModeEnum.SELECT_WITH_GROUPS: return "Select Recipients";
                    case chlk.models.recipients.SelectorModeEnum.SELECT_WITHOUT_GROUPS: return "Create Group";
                    case chlk.models.recipients.SelectorModeEnum.EDIT_WITHOUT_GROUPS: return "Edit Group - " + this.getGroupName();
                    case chlk.models.recipients.SelectorModeEnum.VIEW_WITH_GROUPS: return "People";
                }
            }
        ])
});