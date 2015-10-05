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
        [chlk.controllers.MessagingEnabled],
        'MessageController', EXTENDS(chlk.controllers.BaseController), [

            [ria.mvc.Inject],
            chlk.services.MessageService, 'messageService',

            [ria.mvc.Inject],
            chlk.services.PersonService, 'personService',

            [chlk.controllers.SidebarButton('messages')],
            [[Boolean, Boolean, String, String, Number]],
            function pageAction(postback_, inbox_, role_, keyword_, start_) {
                inbox_ = inbox_ || false;
                var result = this.getMessages_(inbox_, role_, keyword_, start_);
                return this.PushOrUpdateView(chlk.activities.messages.MessageListPage, result);
            },
            [[Boolean, String, String, Number]],
            ria.async.Future, function getMessages_(inbox_, role_, keyword_, start_){
                role_ = role_ || null;
                keyword_ = keyword_ || null;

                return this.messageService
                    .getMessages(start_ | 0, null, inbox_, role_, keyword_)
                    .attach(this.validateResponse_())
                    .then(function(model){
                        this.getContext().getSession().set(ChlkSessionConstants.CURRENT_MESSAGES, model.getItems());
                        return this.convertModel(model, inbox_, role_, keyword_, start_ || 0);
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
                            return this.getMessages_(model.isInbox(), model.getRole(), model.getKeyword(), model.getStart());
                        }, this);
                }else{
                    res = this.getMessages_(model.isInbox(), model.getRole(), model.getKeyword(), 0);
                }
                return  this.PushOrUpdateView(chlk.activities.messages.MessageListPage, res);
            },

            [[chlk.models.common.PaginatedList, Boolean, String, String, Number]],
            function convertModel(list_, inbox_, role_, keyword_, start_)
            {
                var result = new chlk.models.messages.MessageList();
                result.setMessages(list_);
                result.setInbox(inbox_);
                result.setRole(role_);
                result.setKeyword(keyword_);
                result.setStart(start_);
                var messagingSetting = this.getContext().getSession().get(ChlkSessionConstants.MESSAGING_SETTINGS, null);
                result.setDisabledMessaging(this.getCurrentRole().isStudent() && !messagingSetting.isAllowedForStudents() && !messagingSetting.isAllowedForTeachersToStudents());

                return new ria.async.DeferredData(result);
            },

            [chlk.controllers.SidebarButton('messages')],
            [[Boolean, chlk.models.id.MessageId]],
            function sendPageAction(isInbox, replayOnId_)
            {
                var res;
                if (replayOnId_) {
                    res = this.getMessageFromSession(replayOnId_, isInbox);
                    res = res.then(function(model){
                        if(model.getSender()){
                            model = new ria.async.DeferredData(new chlk.models.messages.Message(
                                isInbox,
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
                    res = new ria.async.DeferredData(new chlk.models.messages.Message(isInbox));
                return this.ShadeView(chlk.activities.messages.AddDialog, res);
            },

            [chlk.controllers.SidebarButton('people')],
            [[Boolean, chlk.models.id.SchoolPersonId, String, String]],
            function sendToPersonAction(isInbox, personId, firstName, lastName)
            {

                var model = new ria.async.DeferredData(new chlk.models.messages.Message(
                    isInbox,
                    null,
                    null,
                    new chlk.models.people.User(firstName, lastName, personId)
                ));
                return this.ShadeView(chlk.activities.messages.AddDialog, model);
            },

            [chlk.controllers.SidebarButton('messages')],
            [[chlk.models.messages.SendMessage]],
            function sendAction(model)
            {
                this.view.getCurrent().close();
                var result = this.messageService
                    .send(model)
                    .attach(this.validateResponse_())
                    .then(function(x){
                        return this.getMessages_(model.isInbox());
                    }, this);
                return this.UpdateView(chlk.activities.messages.MessageListPage, result);
            },

            [chlk.controllers.SidebarButton('messages')],
            [[chlk.models.id.MessageId, Boolean]],
            function viewPageAction(id, isInbox)
            {
                var res = this.getMessageFromSession(id, isInbox)
                    .then(function(model){
                        var isReplay = model.getRecipientPerson() && this.getCurrentPerson().getId() == model.getRecipientPerson().getId();
                        model.setReplay(isReplay);
                        model.setInbox(isInbox);
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

            function getMessageFromSession(id, income)
            {
                var res = this.getContext().getSession().get(ChlkSessionConstants.CURRENT_MESSAGES, []).
                    filter(function(message){
                        return message.getId() == id;
                    })[0];
                if (res)
                    return new ria.async.DeferredData(res);
                return this.messageService
                    .getMessage(id, income)
                    .attach(this.validateResponse_());
            }
        ])
});

