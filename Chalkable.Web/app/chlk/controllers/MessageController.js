REQUIRE('chlk.controllers.BaseController');
REQUIRE('chlk.models.messages.Message');
REQUIRE('chlk.models.messages.SendMessage');
REQUIRE('chlk.models.messages.MessageList');
REQUIRE('chlk.activities.messages.AddDialog');
REQUIRE('chlk.activities.messages.ViewDialog');
REQUIRE('chlk.models.id.MessageId');
REQUIRE('chlk.services.MessageService');
REQUIRE('chlk.services.PersonService');
REQUIRE('chlk.activities.messages.MessageListPage');
REQUIRE('chlk.models.common.PaginatedList');

NAMESPACE('chlk.controllers', function (){

    /** @class chlk.controllers.MessageController */
    CLASS(
        'MessageController', EXTENDS(chlk.controllers.BaseController), [

            [ria.mvc.Inject],
            chlk.services.MessageService, 'messageService',

            [ria.mvc.Inject],
            chlk.services.PersonService, 'personService',

            [chlk.controllers.SidebarButton('messages')],
            [[Boolean, Boolean, String, String, Number]],
            function pageAction(postback_, inbox_, role_, keyword_, start_) {

                var result = this.getMessages_(inbox_, role_, keyword_, start_);
                return postback_ ?
                    this.UpdateView(chlk.activities.messages.MessageListPage, result) :
                    this.PushView(chlk.activities.messages.MessageListPage, result);
            },
            [[Boolean, String, String, Number]],
            ria.async.Future, function getMessages_(inbox_, role_, keyword_, start_){
                inbox_ = inbox_ !== false;
                role_ = role_ || null;
                keyword_ = keyword_ || null;

                return this.messageService
                    .getMessages(start_ | 0, null, inbox_, role_, keyword_)
                    .attach(this.validateResponse_())
                    .then(function(model){
                        this.getContext().getSession().set('currentMessages', model.getItems());
                        return this.convertModel(model, inbox_, role_, keyword_);
                    }, this);
            },

            [chlk.controllers.SidebarButton('messages')],
            [[chlk.models.messages.MessageList]],
            function doAction(model) {
                if (model.getSubmitType() == "search")
                    return  this.pageAction(true, model.isInbox(), model.getRole(), model.getKeyword(), 0);
                var res;
                if (model.getSubmitType() == "delete")
                    res = this.messageService.del(model.getSelectedIds());
                if (model.getSubmitType() == "markAsRead")
                    res = this.messageService.markAs(model.getSelectedIds(), true);
                if (model.getSubmitType() == "markAsUnread")
                    res = this.messageService.markAs(model.getSelectedIds(), false);
                if(res){
                    res = res.attach(this.validateResponse_())
                        .then(function(x){
                            return this.getMessages_(model.isInbox(), model.getRole(), model.getKeyword(), 0);
                        }, this);
                    return  this.UpdateView(chlk.activities.messages.MessageListPage, res);
                }
            },

            [[chlk.models.common.PaginatedList, Boolean, String, String]],
            function convertModel(list_, inbox_, role_, keyword_)
            {
                var result = new chlk.models.messages.MessageList();
                result.setMessages(list_);
                result.setInbox(inbox_);
                result.setRole(role_);
                result.setKeyword(keyword_);

                return new ria.async.DeferredData(result);
            },

            [chlk.controllers.SidebarButton('messages')],
            [[chlk.models.id.MessageId]],
            function sendPageAction(replayOnId_)
            {
                var res;
                if (replayOnId_) {
                    res = this.getMessageFromSession(replayOnId_);
                    res.then(function(model){
                        if(this.getContext().getSession().get('currentPerson').getId() == model.getRecipient().getId()){
                            model = new ria.async.DeferredData(new chlk.models.messages.Message(
                                model.getBody(),
                                model.getSubject(),
                                model.getSender(),
                                model.getSent()
                            ));
                        }
                        return model;
                    }, this);

                }
                else
                    res = new ria.async.DeferredData(new chlk.models.messages.Message());
                return this.ShadeView(chlk.activities.messages.AddDialog, res);
            },

            [[chlk.models.id.SchoolPersonId, String, String]],
            function sendToPersonAction(personId, firstName, lastName)
            {

                var model = new ria.async.DeferredData(new chlk.models.messages.Message(
                    null,
                    null,
                    new chlk.models.people.User(firstName, lastName, personId)
                ));
                return this.ShadeView(chlk.activities.messages.AddDialog, model);
            },

            [[chlk.models.messages.SendMessage]],
            function sendAction(model)
            {
                this.messageService
                    .send(model)
                    .attach(this.validateResponse_())
                    .then(function(x){
                        this.view.getCurrent().close();
                        this.pageAction(true);
                    }, this);
            },

            [chlk.controllers.SidebarButton('messages')],
            [[chlk.models.id.MessageId]],
            function viewPageAction(id)
            {
                var res = this.getMessageFromSession(id)
                    .then(function(model){
                        var isReplay = this.getCurrentPerson().getId() == model.getRecipient().getId();
                        model.setReplay(isReplay);
                        if(isReplay && !model.isRead()){
                            return this.messageService.markAs(id.valueOf().toString(), true)
                                .then(function(isRead){
                                    model.setRead(isRead);
                                    return model;
                                }, this);
                        }
                        return model;
                    }, this);
                return this.ShadeView(chlk.activities.messages.ViewDialog, res);
            },

            function getMessageFromSession(id)
            {
                var res = this.getContext().getSession().get('currentMessages', []).
                    filter(function(message){
                        return message.getId() == id;
                    })[0];
                if (res)
                    return new ria.async.DeferredData(res);
                return this.messageService
                    .getMessage(id)
                    .attach(this.validateResponse_());
            }
        ])
});

