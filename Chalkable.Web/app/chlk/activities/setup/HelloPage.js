REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.setup.Hello');

NAMESPACE('chlk.activities.setup', function () {

    /** @class chlk.activities.setup.HelloPage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.setup.Hello)],
        'HelloPage', EXTENDS(chlk.activities.lib.TemplatePage), [
            [ria.mvc.DomEventBind('keyup', '.name-field')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function nameChange(node, event){
                var value = this.dom.find('.salutation').getValue() + " " +
                    this.dom.find('.first-name').getValue() +  " " +
                    this.dom.find('.last-name').getValue();
                this.dom.find('.salutation-label').setValue(value);
            },

            [ria.mvc.DomEventBind('change', '.name-field')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function nameChanged(node, event){
                setTimeout(function(){
                    var el = new ria.dom.Dom('.name-ok');
                    if(this.dom.find('.salutation').hasClass('ok') &&
                        this.dom.find('.first-name').hasClass('ok') &&
                        this.dom.find('.last-name').hasClass('ok')){
                            el.removeClass('x-hidden');
                    }else{
                        el.addClass('x-hidden');
                    }
                }.bind(this), 10);
            },

            OVERRIDE, VOID, function onStop_() {
                BASE();
                ria.dom.Dom('body').removeClass('setup');
            },

            OVERRIDE, VOID, function onRender_(model){
                BASE(model);
                ria.dom.Dom('body').addClass('setup');
            }
        ]);
});