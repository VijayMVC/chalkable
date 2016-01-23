/**
 * Created by Volodymyr on 1/21/2016.
 */

REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.activities.lib.TemplateDialog');

NAMESPACE('chlk.activities.apps', function () {


    /** @class chlk.activities.apps.AppShadeDialogModel */
    CLASS(
        'AppShadeDialogModel', [
            ria.dom.Dom, 'iframe',
            String, 'id',

            function $fromData(iframe, id) {
                BASE();
                this.iframe = iframe;
                this.id = id || null;
            }
        ]);

    /** @class chlk.activities.apps.AppShadeDialogTpl */
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/apps/AppShadeDialogTpl.jade')],
        [ria.templates.ModelBind(chlk.activities.apps.AppShadeDialogModel)],
        'AppShadeDialogTpl', EXTENDS(chlk.templates.ChlkTemplate), [
        ]);

    /** @class chlk.activities.apps.AppShadeDialog */
    CLASS(
        [ria.mvc.DomAppendTo('#chlk-dialogs')],
        [ria.mvc.TemplateBind(chlk.activities.apps.AppShadeDialogTpl)],
        'AppShadeDialog', EXTENDS(chlk.activities.lib.TemplateDialog), [

            function $() {
                BASE();

                this._iframe = null;
                this._id = null;
            },

            [ria.mvc.PartialUpdateRule(null, 'pop-me')],
            VOID, function doUpdateClasses(tpl, model, msg_) {
                if (this._iframe.valueOf()[0] === model.getIframe().valueOf()[0]
                    && this._id === model.getId())
                    this.close();
            },

            OVERRIDE, VOID, function onRender_(model) {

                this.dom.removeClass(this._modelWaitClass);

                if (this._loaderTimer) {
                    this._loaderTimer.cancel();
                    this._loaderTimer = null;
                }

                this._id = model.getId();
                this._iframe = model.getIframe();
                var $iframe = this._iframe.$;

                var offset = $iframe.offset();

                this._iframe.updateCss({
                    position: 'absolute',
                    top: offset.y,
                    left: offset.x,
                    width: $iframe.width(),
                    height: $iframe.height(),
                    'z-index': 10000
                });
            },

            OVERRIDE, VOID, function onResume_() {
                BASE();
                this._iframe && this._iframe.setCss('z-index', 1005);
            },

            OVERRIDE, VOID, function onPause_() {
                this._iframe && this._iframe.setCss('z-index', 998);
                BASE();
            },

            OVERRIDE, VOID, function onStop_() {
                this._iframe.updateCss({
                    position: '',
                    top: '',
                    left: '',
                    width: '',
                    'z-index': ''
                });
                BASE();
            }
        ]);
});
