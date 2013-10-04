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

              function getInnerDocument(){
                  var iframe = this.dom.find('iframe');
                  return jQuery(iframe.valueOf()).get(0).contentWindow;
              },

              function getFrameUrl(splitBy){
                  var iframe = this.dom.find('iframe');
                  return (iframe.getAttr('src') || "").split(splitBy)[0];
              },

              [ria.mvc.DomEventBind('click', '#add-app')],
              [[ria.dom.Dom, ria.dom.Event]],
              VOID, function addApp(node, event){

                  var rUrl = this.getFrameUrl('edit');
                  var announcementAppId = node.getData('announcement-app-id');
                  var announcementId = node.getData('announcement-id');
                  var data = {
                      attach: true,
                      announcementAppId: announcementAppId,
                      announcementId: announcementId
                  };
                  (new chlk.AppApiHost()).addApp(this.getInnerDocument(), rUrl, data);
              },

              [ria.mvc.DomEventBind('click', '#save-app')],
              [[ria.dom.Dom, ria.dom.Event]],
              VOID, function saveApp(node, event){

                  var rUrl = this.getFrameUrl('view');
                  var data = {attach: false};
                  (new chlk.AppApiHost()).addApp(this.getInnerDocument(), rUrl, data);
              }

          ]);
});
