REQUIRE('chlk.templates.announcement.AnnouncementQnAs');
REQUIRE('chlk.activities.lib.TemplatePage');

NAMESPACE('chlk.activities.announcement', function () {


    /** @class chlk.activities.announcement.AnnouncementChatPage*/
    CLASS(
        [ria.mvc.DomAppendTo('body')],
        [chlk.activities.lib.BodyClass('chat')],
        [ria.mvc.ActivityGroup('ChatPopUp')],
        [ria.mvc.TemplateBind(chlk.templates.announcement.AnnouncementQnAs)],
        [ria.mvc.PartialUpdateRule(chlk.templates.announcement.AnnouncementQnAs, '', null, ria.mvc.PartialUpdateRuleActions.Replace)],
        'AnnouncementChatPage', EXTENDS(chlk.activities.lib.TemplatePage), [

            [ria.mvc.DomEventBind('click', '.close-btn')],
            [[ria.dom.Dom, ria.dom.Event]],
            Boolean, function onCloseBtnClick(node, event) {
                this.close();
                return false;
            },

            [ria.mvc.DomEventBind('keypress', '.add-question')],
            [[ria.dom.Dom, ria.dom.Event]],
            function enterClick(node, event){
                if(event.which == ria.dom.Keys.ENTER.valueOf()){
                    node.parent('form').trigger('submit');
                }
            },

            [ria.mvc.DomEventBind('click', '.edit-answer-link, .edit-question-link')],
            [[ria.dom.Dom, ria.dom.Event]],
            function editAnswerClick(node, event){
                var row = node.parent('.row');
                row.find('.edit-answer-text, .edit-question-text, .edit-question-link, .edit-answer-link').fadeOut(function(){
                    var node = row.find('.edit-answer-input, .edit-question-input');
                    node.fadeIn(function(){
                        node.trigger('focus');
                    });
                });
            },

            [ria.mvc.DomEventBind('blur', '.edit-answer-input, .edit-question-input')],
            [[ria.dom.Dom, ria.dom.Event]],
            function blurAnswer(node, event){
                var row = node.parent('.row');
                if(row.find('.edit-answer-text').exists())
                    row.find('.edit-answer-input, .edit-question-input').fadeOut(function(){
                        setTimeout(function(){
                            row.find('.edit-answer-text, .edit-question-text, .edit-question-link, .edit-answer-link').fadeIn();
                        }, 500);

                    });
            }

        ]
    );
});