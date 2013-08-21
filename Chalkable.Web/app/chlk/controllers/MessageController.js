REQUIRE('chlk.controllers.BaseController');
REQUIRE('chlk.models.messages.Message');
REQUIRE('chlk.models.messages.MessageList');
REQUIRE('chlk.models.id.MessageId');
REQUIRE('chlk.services.MessageService');
REQUIRE('chlk.activities.messages.MessageListPage');
REQUIRE('chlk.models.common.PaginatedList');

NAMESPACE('chlk.controllers', function (){

    /** @class chlk.controllers.MessageController */
    CLASS(
        'MessageController', EXTENDS(chlk.controllers.BaseController), [

            [ria.mvc.Inject],
            chlk.services.MessageService, 'messageService',

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
                        return this.convertModel(model, inbox_, role_, keyword_);
                    }.bind(this));
                return postback_ ?
                    this.UpdateView(chlk.activities.messages.MessageListPage, result) :
                    this.PushView(chlk.activities.messages.MessageListPage, result);
            },

            [chlk.controllers.SidebarButton('message')],
            [[chlk.models.messages.MessageList]],
            function searchPageAction(model) {
                return this.pageAction(true, model.isInbox(), model.getRole(), model.getKeyword(), 0);
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
            }
        ])
});

