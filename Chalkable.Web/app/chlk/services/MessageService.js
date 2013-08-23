REQUIRE('chlk.services.BaseService');
REQUIRE('ria.async.Future');
REQUIRE('chlk.models.messages.Message');
REQUIRE('chlk.models.messages.SendMessage');
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
            },

            [[String, Boolean]],
            ria.async.Future, function markAs(ids, read) {
                return this.get('PrivateMessage/MarkAsRead.json', null, {
                    ids: ids,
                    read: read
                });
            },

            [[String]],
            ria.async.Future, function del(ids) {
                return this.get('PrivateMessage/Delete.json', null,{
                    ids: ids
                });
            },

            [[chlk.models.messages.SendMessage]],
            ria.async.Future, function send(model) {
                return this.get('PrivateMessage/Send.json', null,{
                    personId: model.getRecipientId().valueOf(),
                    subject: model.getSubject(),
                    body: model.getBody()
                });
            }
        ])
});
