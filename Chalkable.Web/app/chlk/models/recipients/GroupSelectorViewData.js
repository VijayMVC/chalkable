REQUIRE('chlk.models.recipients.GroupsListViewData');
REQUIRE('chlk.models.recipients.UsersListViewData');
REQUIRE('chlk.models.group.Group');
REQUIRE('chlk.models.people.ShortUserInfo');

NAMESPACE('chlk.models.recipients', function () {
    "use strict";

    /** @class chlk.models.recipients.GroupSelectorViewData*/
    CLASS(
        'GroupSelectorViewData', EXTENDS(chlk.models.recipients.BaseViewData), [
            chlk.models.recipients.GroupsListViewData, 'groupsPart',
            chlk.models.recipients.UsersListViewData, 'myStudentsPart',
            chlk.models.recipients.UsersListViewData, 'allStudentsPart',
            ArrayOf(chlk.models.group.Group), 'selectedGroups',
            ArrayOf(chlk.models.people.ShortUserInfo), 'selectedStudents',
            String, 'selectedItems',
            String, 'requestId',
            chlk.models.group.Group, 'groupInfo',
            Boolean, 'hasAccessToLE',
            chlk.models.classes.ClassesForTopBar, 'topData',

            [[String, chlk.models.recipients.SelectorModeEnum, Boolean, Boolean, chlk.models.recipients.GroupsListViewData, chlk.models.recipients.UsersListViewData,
                chlk.models.recipients.UsersListViewData, ArrayOf(chlk.models.group.Group), ArrayOf(chlk.models.people.ShortUserInfo), chlk.models.group.Group,
                Boolean, chlk.models.classes.ClassesForTopBar]],
            function $(requestId_, selectorMode_, hasAccessToAllStudents_, hasOwnStudents_, groupsPart_, myStudentsPart_, allStudentsPart_,
                       selectedGroups_, selectedStudents_, groupInfo_, hasAccessToLE_, topData_){
                BASE(selectorMode_, hasAccessToAllStudents_, hasOwnStudents_);
                requestId_ && this.setRequestId(requestId_);
                groupsPart_ && this.setGroupsPart(groupsPart_);
                myStudentsPart_ && this.setMyStudentsPart(myStudentsPart_);
                allStudentsPart_ && this.setAllStudentsPart(allStudentsPart_);
                selectedGroups_ && this.setSelectedGroups(selectedGroups_);
                selectedStudents_ && this.setSelectedStudents(selectedStudents_);
                groupInfo_ && this.setGroupInfo(groupInfo_);
                hasAccessToLE_ && this.setHasAccessToLE(hasAccessToLE_);
                topData_ && this.setTopData(topData_);
            }
        ]);
});
