REQUIRE('chlk.activities.lib.TemplateDialog');
REQUIRE('chlk.templates.apps.AppWrapperDialogTpl');
REQUIRE('chlk.AppApiHost');
 
 NAMESPACE('chlk.activities.apps', function () {
     /** @class chlk.activities.apps.AppWrapperDialog*/

      var BLACK_CLASS = 'black';

      CLASS(
        [ria.mvc.ActivityGroup('AppWrapperDialog')],
        [ria.mvc.DomAppendTo('#chlk-dialogs')],
        [ria.mvc.TemplateBind(chlk.templates.apps.AppWrapperDialogTpl)],
        'AppWrapperDialog', EXTENDS(chlk.activities.lib.TemplateDialog), [

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
                  var data = {
                      attach: true,
                      announcementAppId: announcementAppId
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

              OVERRIDE, VOID, function onResume_() {
                  BASE();
                  this._overlay.addClass(BLACK_CLASS);
              },

              OVERRIDE, VOID, function onPause_() {
                  BASE();

              },

              OVERRIDE, VOID, function onStop_() {
                  BASE();
                  this._overlay.removeClass(BLACK_CLASS);
                  /*
                   setTimeout(function(){
                   jQuery('.grade-input:visible').focus().select();
                   }, 100);
                   */
              }
          ]);
});
