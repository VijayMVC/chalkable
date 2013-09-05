REQUIRE('chlk.activities.lib.TemplateDialog');
REQUIRE('chlk.templates.apps.AppWrapperDialog');
REQUIRE('chlk.AppApiHost');
 
 NAMESPACE('chlk.activities.apps', function () {
     /** @class chlk.activities.apps.AppWrapperDialog*/

      var BLACK_CLASS = 'black';

      CLASS(
        [ria.mvc.ActivityGroup('AppWrapperDialog')],
        [ria.mvc.DomAppendTo('#chlk-dialogs')],
        [ria.mvc.TemplateBind(chlk.templates.apps.AppWrapperDialog)],
        'AppWrapperDialog', EXTENDS(chlk.activities.lib.TemplateDialog), [
              function $() {
                  BASE();
              },

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
                  var announcementId = node.getData('announcement-id');
                  var appId = node.getData('app-id');
                  var data = {
                      attach: true,
                      appId: appId,
                      announcementId : announcementId
                  };
                  (new chlk.AppApiHost()).addApp(this.getInnerDocument(), rUrl, data);
              },

              [ria.mvc.DomEventBind('click', '#save-app')],
              [[ria.dom.Dom, ria.dom.Event]],
              VOID, function saveApp(node, event){

                  var rUrl = this.getFrameUrl('view');
                  var data = {attach: false};
                  (new chlk.AppApiHost()).addApp(this.getInnerDocument(), rUrl, data);
              },

              [[Object]],
              OVERRIDE, VOID, function onRender_(data) {
                  BASE(data);
              },

              OVERRIDE, VOID, function onResume_() {
                  BASE();
                  this._overlay.addClass(BLACK_CLASS);
              },

              OVERRIDE, VOID, function onPause_() {
                  BASE();
                  this._overlay.removeClass(BLACK_CLASS);
              },

              OVERRIDE, VOID, function onStop_() {
                  BASE();

                  /*
                   setTimeout(function(){
                   jQuery('.grade-input:visible').focus().select();
                   }, 100);
                   */
              }
          ]);
});
