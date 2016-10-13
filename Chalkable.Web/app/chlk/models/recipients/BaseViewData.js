NAMESPACE('chlk.models.recipients', function () {
    "use strict";

    /** @class chlk.models.recipients.SelectorModeEnum*/
    ENUM('SelectorModeEnum', {
        SELECT_WITH_GROUPS: 0,
        SELECT_WITHOUT_GROUPS: 1,
        EDIT_WITHOUT_GROUPS: 2,
        VIEW_WITH_GROUPS: 3
    });

    /** @class chlk.models.recipients.RecipientTypeEnum*/
    ENUM('RecipientTypeEnum', {
        GROUP: 0,
        STUDENT: 1
    });

    /** @class chlk.models.recipients.BaseViewData*/
    CLASS(
        'BaseViewData', [
            Boolean, 'hasAccessToAllStudents',
            Boolean, 'hasOwnStudents',
            chlk.models.recipients.SelectorModeEnum, 'selectorMode',

            function hasGroups(){
                return this.getSelectorMode() == chlk.models.recipients.SelectorModeEnum.SELECT_WITH_GROUPS ||
                    this.getSelectorMode() == chlk.models.recipients.SelectorModeEnum.VIEW_WITH_GROUPS
            },

            function getSelectorCSSClass(){
                var mode = this.getSelectorMode();
                switch (mode){
                    case chlk.models.recipients.SelectorModeEnum.SELECT_WITH_GROUPS: return "select-with-groups";
                    case chlk.models.recipients.SelectorModeEnum.SELECT_WITHOUT_GROUPS: return "select-without-groups";
                    case chlk.models.recipients.SelectorModeEnum.EDIT_WITHOUT_GROUPS: return "edit-without-groups";
                    case chlk.models.recipients.SelectorModeEnum.VIEW_WITH_GROUPS: return "view-with-groups";
                }
            },

            function getSubmitButtonText(){
                var mode = this.getSelectorMode();
                switch (mode){
                    case chlk.models.recipients.SelectorModeEnum.SELECT_WITH_GROUPS: return "Add";
                    case chlk.models.recipients.SelectorModeEnum.SELECT_WITHOUT_GROUPS: return "Create";
                    case chlk.models.recipients.SelectorModeEnum.EDIT_WITHOUT_GROUPS: return "Save";
                }
            },

            [[chlk.models.recipients.SelectorModeEnum, Boolean, Boolean]],
            function $(selectorMode_, hasAccessToAllStudents_, hasOwnStudents_){
                BASE();
                selectorMode_ && this.setSelectorMode(selectorMode_);
                hasAccessToAllStudents_ && this.setHasAccessToAllStudents(hasAccessToAllStudents_);
                hasOwnStudents_ && this.setHasOwnStudents(hasOwnStudents_);
            }
        ]);
});
