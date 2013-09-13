REQUIRE('chlk.models.common.PaginatedList');
REQUIRE('chlk.models.people.UsersListSubmit');

NAMESPACE('chlk.models.people', function () {
    "use strict";

    /** @class chlk.models.people.UsersList*/
    CLASS(
        'UsersList', EXTENDS(chlk.models.people.UsersListSubmit),[
            chlk.models.common.PaginatedList, 'users',
            Number, 'roleId',
            String, 'gradeLevelsIds',

            [[chlk.models.common.PaginatedList, Boolean, Number,  String, Number]],
            function $(users_, byLastName_, selectedIndex_,  filter_, start_){
                BASE();
                if(users_)
                   this.setUsers(users_);
                this.setSelectedIndex(selectedIndex_ || 0);
                this.setByLastName(byLastName_ || false);
                if(filter_)
                    this.setFilter(filter_);
                this.setStart(start_ || 0);
            }
        ]);
});
