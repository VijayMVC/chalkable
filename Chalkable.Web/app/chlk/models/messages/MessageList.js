REQUIRE('chlk.models.common.ChlkDate');
REQUIRE('chlk.models.id.MessageId');
REQUIRE('chlk.models.messages.Message');
REQUIRE('chlk.models.people.User');
REQUIRE('chlk.models.common.PaginatedList');


NAMESPACE('chlk.models.messages', function () {

    "use strict";
    /** @class chlk.models.messages.MessageList*/
    CLASS(
        'MessageList', [
            chlk.models.common.PaginatedList, 'messages',
            Boolean, 'inbox',
            String, 'role',
            String, 'keyword',
            String, 'selectedIds',
            String, 'submitType'
        ]);
});
