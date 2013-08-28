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

            [chlk.controllers.SidebarButton('message')],
            [[Boolean, Boolean, String, String, Number]],
            function pageAction(postback_, inbox_, role_, keyword_, start_) {
                inbox_ = inbox_ !== false;
                role_ = role_ || null;
                keyword_ = keyword_ || null;

                var result = this.messageService
                    .getMessages(start_ | 0, null, inbox_, role_, keyword_)
                    .attach(this.validateResponse_())
                    .then(function(model){
                        this.getContext().getSession().set('currentMessages', model.getItems());
                        return this.convertModel(model, inbox_, role_, keyword_);
                    }.bind(this));
                return postback_ ?
                    this.UpdateView(chlk.activities.messages.MessageListPage, result) :
                    this.PushView(chlk.activities.messages.MessageListPage, result);
            },

            [chlk.controllers.SidebarButton('message')],
            [[chlk.models.messages.MessageList]],
            function doAction(model) {
                if (model.getSubmitType() == "search")
                    return this.pageAction(true, model.isInbox(), model.getRole(), model.getKeyword(), 0);
                if (model.getSubmitType() == "delete")
                    this.messageService.del(model.getSelectedIds())
                        .then(function(x){
                            this.pageAction(true, model.isInbox(), model.getRole(), model.getKeyword(), 0);
                        }.bind(this));
                if (model.getSubmitType() == "markAsRead")
                    this.messageService.markAs(model.getSelectedIds(), true)
                        .then(function(x){
                            this.pageAction(true, model.isInbox(), model.getRole(), model.getKeyword(), 0);
                        }.bind(this));
                if (model.getSubmitType() == "markAsUnread")
                    this.messageService.markAs(model.getSelectedIds(), false)
                        .then(function(x){
                            this.pageAction(true, model.isInbox(), model.getRole(), model.getKeyword(), 0);
                        }.bind(this));

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

            [[String]],
            function sendPageAction(replayOnId_)
            {
                var message;
                if (replayOnId_)
                {
                    message = this.getMessageFromSession(replayOnId_);
                }
                else
                    message = new chlk.models.messages.Message();
                model = ria.async.DeferredData(message);
                return this.ShadeView(chlk.activities.messages.AddDialog, model);
            },

            [[chlk.models.messages.SendMessage]],
            function sendAction(model)
            {
                this.messageService.send(model)
                    .then(function(x){
                        this.view.getCurrent().close();
                        this.pageAction(true);
                    }.bind(this));
            },

            [[String]],
            function viewPageAction(id)
            {
                var message = this.getMessageFromSession(id);
                var model = ria.async.DeferredData(message);
                return this.ShadeView(chlk.activities.messages.ViewDialog, model);
            },

            function getMessageFromSession(id)
            {
                return this.getContext().getSession().get('currentMessages', []).
                    filter(function(message){
                        return message.getId().valueOf() == id;
                    })[0];
            }
        ])
});

