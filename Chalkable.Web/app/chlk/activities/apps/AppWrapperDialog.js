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
                  this.onAttachClick_();
              },


              [ria.mvc.DomEventBind('click', '.close')],
              OVERRIDE, Boolean, function onCloseBtnClick(node, event){
                  var isSave = this.dom.find('#save-app').exists();
                  var isAppAttach = this.dom.find('#add-app').exists();
                  if (isSave || (!isSave && !isAppAttach)){
                      this.close();
                  }
                  else{
                     (new chlk.AppApiHost()).closeApp({});
                  }
                  return false;
              },

              [[ria.dom.Dom]],
              VOID, function onAttachClick_(){
                  var isAppAttach = this.dom.find('#add-app').exists();
                  var isSave = this.dom.find('#save-app').exists();

                  if (!isAppAttach && !isSave){
                      this.close();
                  }

                  var rUrl = null;
                  var data = null;

                  if (isAppAttach){
                      rUrl = this.getFrameUrl('edit');
                      var node = this.dom.find('#add-app');
                      var announcementAppId = node.getData('announcement-app-id');
                      var announcementId = node.getData('announcement-id');
                      data = {
                          attach: true,
                          announcementAppId: announcementAppId,
                          announcementId: announcementId
                      };

                  } else if(isSave){
                      rUrl = this.getFrameUrl('view');
                      data = {attach: false};
                  }
                  (new chlk.AppApiHost()).addApp(this.getInnerDocument(), rUrl, data);

              },

              [ria.mvc.DomEventBind('click', '#save-app')],
              [[ria.dom.Dom, ria.dom.Event]],
              VOID, function saveApp(node, event){
                  this.onAttachClick_();
              }

          ]);
});
