REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.apps.AppWrapperPageTpl');

NAMESPACE('chlk.activities.apps', function () {

    /** @class chlk.activities.apps.AppWrapperPage*/
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.apps.AppWrapperPageTpl)],
        'AppWrapperPage', EXTENDS(chlk.activities.lib.TemplatePage), [

            [[Object]],
            OVERRIDE, VOID, function onRefresh_(data) {
                BASE(data);
                this.dom.find('iframe').$
                    //.css({height: ria.dom.Dom('#main').$.parent().height() - 27*2 + 'px'})
                    .load(function () {
                        this.dom.find('iframe').parent()
                            .removeClass('partial-update');
                    }.bind(this))
            }
        ]);
});
