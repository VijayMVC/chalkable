REQUIRE('chlk.models.common.PaginatedList');
REQUIRE('chlk.models.people.UsersListSubmit');
REQUIRE('chlk.models.common.Role');
REQUIRE('chlk.models.people.User');

NAMESPACE('chlk.models.people', function () {
    "use strict";

    /** @class chlk.models.people.UsersList*/
    CLASS(
        'UsersList', EXTENDS(chlk.models.people.UsersListSubmit),[
            chlk.models.common.PaginatedList, 'users',

            String, 'gradeLevelsIds',

            String, 'rolesText',

            Boolean, 'hasAccess',

            chlk.models.common.Role, 'currentUserRole',
            chlk.models.people.User, 'currentUser',

            [[chlk.models.common.PaginatedList, Boolean, Number,  String
                , Number, chlk.models.common.Role, chlk.models.people.User, String, Boolean]],
            function $(users_, byLastName_, selectedIndex_,  filter_, start_, currentUserRole_, currentUser_, rolesText_, hasAccess_){
                BASE();
                if(users_)
                   this.setUsers(users_);
                this.setSelectedIndex(selectedIndex_ || 0);
                this.setByLastName(byLastName_ || false);
                if(filter_)
                    this.setFilter(filter_);
                if(hasAccess_)
                    this.setHasAccess(hasAccess_);
                if(rolesText_)
                    this.setRolesText(rolesText_);
                this.setStart(start_ || 0);

                if(currentUserRole_)
                    this.setCurrentUserRole(currentUserRole_);
                if(currentUser_)
                    this.setCurrentUser(currentUser_);
            }
        ]);
});
