REQUIRE('chlk.services.BaseService');
REQUIRE('ria.async.Future');
REQUIRE('chlk.models.messages.Message');
REQUIRE('chlk.models.id.MessageId');




NAMESPACE('chlk.services', function () {
    "use strict";

    /** @class chlk.services.MessageService*/
    CLASS(
        'MessageService', EXTENDS(chlk.services.BaseService), [
            [[Number, Boolean, Boolean, String, String]],
            ria.async.Future, function getMessages(start_, read_, income_, role_, keyword_) {
                return this.getPaginatedList('PrivateMessage/List.json', chlk.models.messages.Message, {
                    start: start_,
                    count: 10,
                    read: read_,
                    income: income_ !== false,
                    role: role_ ? role_ : "",
                    keyword: keyword_
                });
            }
        ])
});
