REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.people.UsersListTpl');
REQUIRE('chlk.templates.people.UsersForGridTpl');
REQUIRE('chlk.templates.people.UsersGridTpl');

NAMESPACE('chlk.activities.person', function () {

    /** @class chlk.activities.person.PersonGrid */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.people.UsersListTpl)],
        'PersonGrid', EXTENDS(chlk.activities.lib.TemplatePage), [
            [ria.mvc.DomEventBind('mouseover mouseleave', '.people-search-img')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function imgHover(node, event){
                this.dom.find('.people-search').toggleClass('hovered');
            },

            [ria.mvc.DomEventBind('focus blur', '.people-search')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function filterFocus(node, event){
                node.toggleClass('hovered');
            },

            [[ria.dom.Dom]],
            VOID, function submitFormWithStart(node){
                var form = node.parent('form');
                form.find('[name=start]').setValue(0);
                form.trigger('submit');
            },

            VOID, function clearSearch(){
                var node = this.dom.find('.people-search');
                node.setValue('');
                this.submitFormWithStart(node);
                this.dom.find('.people-search-img').removeClass('opacity0');
                this.dom.find('.people-search-close').addClass('opacity0');
            },

            [ria.mvc.DomEventBind('keyup', '.people-search')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function filterKeyUp(node, event){
                var value = node.getValue() || '';
                if(value.length > 1){
                    this.submitFormWithStart(node);
                    this.dom.find('.people-search-img').addClass('opacity0');
                    this.dom.find('.people-search-close').removeClass('opacity0');
                }else{
                    if(!value.length)
                        this.clearSearch();
                }
            },

            [ria.mvc.DomEventBind('click', '.first-last:not(.pressed)')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function firstLastClick(node, event){
                var value = node.getData('value');
                this.dom.find('input[name=byLastName]').setValue(value);
                this.dom.find('.first-last.pressed').removeClass('pressed');
                node.addClass('pressed');
                this.submitFormWithStart(node);
            },

            [ria.mvc.DomEventBind('click', '.people-search-close')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function closeSearchClick(node, event){
                this.clearSearch();
            },

            OVERRIDE, VOID, function onPartialRender_(model, msg_){
                BASE(model, msg_);
                if(model.getUsers){
                    var count = model.getUsers().getTotalCount();
                    this.dom.find('.total-count').setHTML( count + ' ' + Msg.Person(count != 1));
                }
            }
        ]);
});