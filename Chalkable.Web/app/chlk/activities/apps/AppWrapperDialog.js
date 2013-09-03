REQUIRE('chlk.activities.lib.TemplateDialog');
REQUIRE('chlk.templates.apps.AppWrapperDialog');
 
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
