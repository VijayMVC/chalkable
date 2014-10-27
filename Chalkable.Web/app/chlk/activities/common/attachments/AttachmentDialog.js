REQUIRE('chlk.activities.lib.TemplateDialog');
REQUIRE('chlk.templates.common.attachments.AttachmentDialogTpl');

NAMESPACE('chlk.activities.common.attachments', function () {
 /** @class chlk.activities.common.attachments.AttachmentDialog*/
  var BLACK_CLASS = 'black';

  CLASS(
    [ria.mvc.ActivityGroup('AttachmentDialog')],
    [ria.mvc.DomAppendTo('#chlk-dialogs')],
    [ria.mvc.TemplateBind(chlk.templates.common.attachments.AttachmentDialogTpl)],
    'AttachmentDialog', EXTENDS(chlk.activities.lib.TemplateDialog), [

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
              jQuery(window).off('resize.attachment');
          },

          function onWindowResize_(){
              var height = this.dom.find('.base-attachment-dialog').height();
              this.dom.find('.picture-container-2').setCss('max-height', height - 22 + 'px');
          },

          OVERRIDE, VOID, function onRender_(model){
              BASE(model);
              this.onWindowResize_();
              var that = this;
              jQuery(window).on('resize.attachment', function(){
                  that.onWindowResize_();
              })
          }
      ]);
});
