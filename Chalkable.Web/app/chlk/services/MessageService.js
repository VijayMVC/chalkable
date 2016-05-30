REQUIRE('chlk.services.BaseService');
REQUIRE('ria.async.Future');
REQUIRE('chlk.models.messages.Message');
REQUIRE('chlk.models.messages.SendMessage');
REQUIRE('chlk.models.id.MessageId');
REQUIRE('chlk.models.messages.RecipientViewData');

NAMESPACE('chlk.services', function () {
    "use strict";

    /** @class chlk.services.MessageService*/
    CLASS(
        'MessageService', EXTENDS(chlk.services.BaseService), [
            [[Number, Boolean, Boolean, String, String, Boolean, Number]],
            ria.async.Future, function getMessages(start_, read_, income_, role_, keyword_, classOnly_, year_) {
                return this.getPaginatedList('PrivateMessage/List.json', chlk.models.messages.Message, {
                    start: start_,
                    count: 10,
                    read: read_,
                    income: income_ !== false,
                    role: role_ ? role_ : "",
                    keyword: keyword_,
                    classOnly: classOnly_ || false,
                    acadYear: year_
                });
            },

            [[chlk.models.id.MessageId, Boolean]],
            ria.async.Future, function getMessage(id, income) {
                return this.get('PrivateMessage/Read.json', chlk.models.messages.Message, {
                    id: id.valueOf(),
                    income: income
                });
            },

            [[String, Boolean]],
            ria.async.Future, function markAs(ids, read) {
                return this.get('PrivateMessage/MarkAsRead.json', null, {
                    ids: ids,
                    read: read
                });
            },

            [[String, Boolean]],
            ria.async.Future, function del(ids, income_) {
                return this.get('PrivateMessage/Delete.json', null,{
                    ids: ids,
                    income: income_ || false
                });
            },

            [[chlk.models.id.SchoolPersonId, chlk.models.id.ClassId]],
            ria.async.Future, function canSendMessage(personId_, classId_) {
                return this.get('PrivateMessage/CanSendMessage.json', Boolean,{
                    personId: personId_ && personId_.valueOf(),
                    classId: classId_ && classId_.valueOf()
                });
            },

            [[String]],
            ria.async.Future, function listPossibleRecipients(filter) {
                return this.get('PrivateMessage/ListPossibleRecipients.json', ArrayOf(chlk.models.messages.RecipientViewData),{
                    filter: filter
                });
            },

            [[chlk.models.messages.SendMessage]],
            ria.async.Future, function send(model) {
                var ids = JSON.parse(model.getRecipientId())
                return this.post('PrivateMessage/Send.json', null,{
                    personId: ids[0],
                    subject: model.getSubject(),
                    body: model.getBody(),
                    classId: ids[1]
                });
            }
        ])
});
