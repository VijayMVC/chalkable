REQUIRE('chlk.activities.lib.TemplateDialog');
REQUIRE('chlk.activities.common.attachments.AttachmentDialog');
REQUIRE('chlk.templates.apps.AppWrapperDialogTpl');
REQUIRE('chlk.AppApiHost');

NAMESPACE('chlk.activities.apps', function () {
    /** @class chlk.activities.apps.AppWrapperDialog*/

    CLASS(
        [ria.mvc.ActivityGroup('AppWrapperDialog')],
        [ria.mvc.DomAppendTo('#chlk-dialogs')],
        [ria.mvc.TemplateBind(chlk.templates.apps.AppWrapperDialogTpl)],
        'AppWrapperDialog', EXTENDS(chlk.activities.common.attachments.AttachmentDialog), [

            function getInnerDocument() {
                var iframe = this.dom.find('iframe');
                return jQuery(iframe.valueOf()).get(0).contentWindow;
            },

            function getFrameUrl(splitBy) {
                var iframe = this.dom.find('iframe');
                return (iframe.getAttr('src') || "").split(splitBy)[0];
            },

            [ria.mvc.DomEventBind('click', '#add-app')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function addApp(node, event) {
                this.onAttachClick_();
            },


            [ria.mvc.DomEventBind('click', '.close')],
            OVERRIDE, Boolean, function onCloseBtnClick(node, event) {
                var isSave = this.dom.find('#save-app').exists();
                var isAppAttach = this.dom.find('#add-app').exists();
                if (isSave || (!isSave && !isAppAttach)) {
                    this.close();
                }
                else {
                    var announcementAppId = this.dom.find('#add-app').getData('announcement-app-id');
                    var simpleApp = !node.getData('advanced-app');
                    (new chlk.AppApiHost()).closeApp({
                        announcementAppId: announcementAppId,
                        simpleApp: simpleApp
                    });
                }
                return false;
            },

            [[ria.dom.Dom]],
            VOID, function onAttachClick_() {
                var isAppAttach = this.dom.find('#add-app').exists();
                var isSave = this.dom.find('#save-app').exists();

                if (!isAppAttach && !isSave) {
                    this.close();
                }

                var rUrl = null;
                var data = null;

                this.dom.find('.iframe-wrap').addClass('partial-update');
                this.dom.find('#save-app').setAttr('disabled', 'disabled');
                this.dom.find('#add-app').setAttr('disabled', 'disabled');

                if (isAppAttach) {
                    rUrl = this.getFrameUrl('edit');
                    var node = this.dom.find('#add-app');
                    var announcementAppId = node.getData('announcement-app-id');
                    var announcementId = node.getData('announcement-id');
                    var announcementType = node.getData('announcement-type');
                    var simpleApp = !node.getData('advanced-app');
                    data = {
                        attach: true,
                        announcementAppId: announcementAppId,
                        announcementId: announcementId,
                        announcementType: announcementType,
                        simpleApp: simpleApp
                    };

                } else if (isSave) {
                    var node = this.dom.find('#save-app');
                    var simpleApp = !node.getData('advanced-app');
                    rUrl = this.getFrameUrl('view');
                    data = {attach: false, simpleApp: simpleApp};
                }
                (new chlk.AppApiHost()).addApp(this.getInnerDocument(), rUrl, data);

            },

            [ria.mvc.PartialUpdateRule(null, 'unfreeze')],
            [[Object, Object, String]],
            VOID, function unfreeze(tpl, model, msg_) {
                this.dom.find('.iframe-wrap').removeClass('partial-update');
                this.dom.find('#save-app').removeAttr('disabled');
                this.dom.find('#add-app').removeAttr('disabled');
            },

            [ria.mvc.DomEventBind('click', '#save-app')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function saveApp(node, event) {
                this.onAttachClick_();
            }

        ]);
});
