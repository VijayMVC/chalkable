REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.templates.PaginatedList');

NAMESPACE('chlk.templates.controls.group_people_selector', function () {

    /** @class chlk.templates.controls.group_people_selector.PersonItemsTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/controls/group-people-selector/person-items.jade')],
        [ria.templates.ModelBind(chlk.models.common.PaginatedList)],
        'PersonItemsTpl', EXTENDS(chlk.templates.PaginatedList), [
            Array, 'selected',

            Boolean, 'hasAccessToLE',

            Boolean, 'messagingDisabled',

            Object, 'messagingSettings',

            chlk.models.recipients.SelectorModeEnum, 'selectorMode',

            [[chlk.models.people.User]],
            Boolean, function canViewInfo(user){
                var currentRole = this.getUserRole();
                var currentUser = this.getCurrentUser();
                return !currentRole || !currentUser || currentUser.getId() == user.getId()
                    || currentRole.isTeacher() || currentRole.isAdmin()
                    || currentRole.isStudent() && user.getRole().getId() == chlk.models.common.RoleEnum.TEACHER.valueOf();
            },

            function showMsgIcon(student)
            {
                var enableMessaging, isStudent = this.getUserRole().isStudent();
                var msgSettings = this.messagingSettings;
                if(isStudent){
                    enableMessaging = msgSettings.isAllowedForStudents() &&
                        (!msgSettings.isAllowedForStudentsInTheSameClass() || student.isClassmate());
                }else{
                    enableMessaging = msgSettings.isAllowedForTeachersToStudents() &&
                        (student.isMystudent() || !msgSettings.isAllowedForTeachersToStudentsInTheSameClass());
                }

                return !this.isMessagingDisabled() && enableMessaging;
            }
        ])
});