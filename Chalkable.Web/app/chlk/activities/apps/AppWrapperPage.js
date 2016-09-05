REQUIRE('ria.async.Completer');

REQUIRE('chlk.AppApiHost');

REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.apps.AppWrapperPageTpl');

NAMESPACE('chlk.activities.apps', function () {

    /** @class chlk.activities.apps.AppWrapperPage*/
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.apps.AppWrapperPageTpl)],
        'AppWrapperPage', EXTENDS(chlk.activities.lib.TemplatePage), [

            Boolean, 'ready',

            [[Object]],
            OVERRIDE, VOID, function onRefresh_(data) {
                BASE(data);
                this.dom.find('iframe').$
                    //.css({height: ria.dom.Dom('#main').$.parent().height() - 27*2 + 'px'})
                    .load(function () {
                        this.ready = false;
                        this.dom.find('iframe').parent()
                            .removeClass('partial-update');
                    }.bind(this))
            },

            function getInnerDocument() {
                var iframe = this.dom.find('iframe');
                return jQuery(iframe.valueOf()).get(0).contentWindow;
            },

            function getFrameUrl(splitBy) {
                var iframe = this.dom.find('iframe');
                return (iframe.getAttr('src') || "").split(splitBy)[0];
            },

            OVERRIDE, Object, function isReadyForClosing() {
                if (this.ready !== false)
                    return true;

                var completer = new ria.async.Completer;

                chlk.AppApiHost().isAppReadyForClosing(
                    this.getInnerDocument(),
                    this.getFrameUrl('myview'),
                    completer.complete);

                return completer.getFuture();
            }
        ]);
});
