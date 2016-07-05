REQUIRE('chlk.templates.announcement.AnnouncementQnAs');
REQUIRE('chlk.activities.lib.TemplatePage');

NAMESPACE('chlk.activities.announcement', function () {


    /** @class chlk.activities.announcement.AnnouncementChatPage*/
    CLASS(
        [ria.mvc.DomAppendTo('body')],
        [chlk.activities.lib.BodyClass('chat')],
        [ria.mvc.ActivityGroup('ChatPopUp')],
        [ria.mvc.TemplateBind(chlk.templates.announcement.AnnouncementQnAs)],
        'AnnouncementChatPage', EXTENDS(chlk.activities.lib.TemplatePage), [

            [ria.mvc.DomEventBind('click', '.close-btn')],
            [[ria.dom.Dom, ria.dom.Event]],
            Boolean, function onCloseBtnClick(node, event) {
                this.close();
                return false;
            }

        ]
    );
});