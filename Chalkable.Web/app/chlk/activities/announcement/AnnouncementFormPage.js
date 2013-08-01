REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.announcement.AnnouncementForm');
REQUIRE('chlk.templates.announcement.Announcement');
REQUIRE('chlk.templates.announcement.LastMessages');
REQUIRE('chlk.templates.class.TopBar');

NAMESPACE('chlk.activities.announcement', function () {

    var handler;

    /** @class chlk.activities.announcement.AnnouncementFormPage*/
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.announcement.AnnouncementForm)],
        [ria.mvc.PartialUpdateRule(chlk.templates.announcement.Announcement, '', '.ann-form-container', ria.mvc.PartialUpdateRuleActions.Replace)],
        [ria.mvc.PartialUpdateRule(chlk.templates.announcement.AnnouncementForm, '', '#main>DIV:visible', ria.mvc.PartialUpdateRuleActions.Replace)],
        [ria.mvc.PartialUpdateRule(chlk.templates.announcement.LastMessages, '', '.drop-down-container', ria.mvc.PartialUpdateRuleActions.Replace)],
        [chlk.activities.lib.PageClass('new-item')],
        'AnnouncementFormPage', EXTENDS(chlk.activities.lib.TemplatePage), [

            [ria.mvc.DomEventBind('click', '.class-button')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function classClick(node, event){
                var classId = node.getAttr('classId');
                this.dom.find('input[name=classId]').setValue(classId);
            },

            [ria.mvc.DomEventBind('click', '.action-button')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function typeClick(node, event){
                var typeId = node.getAttr('typeId');
                var typeName = node.getAttr('typeName');
                this.dom.find('input[name=announcementtypeid]').setValue(typeId);
                this.dom.find('input[name=announcementtypename]').setValue(typeName);
            },

            [ria.mvc.DomEventBind('click', 'form')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function formClick(node, event){

            },

            /*[ria.mvc.DomEventBind('click', '#content')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function focusContent(node, event){
                node.triggerEvent('focus');
            },*/


            [ria.mvc.DomEventBind('focus keydown keyup', '#content')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function showDropDown(node, event){
                if(this.dom.find('[name=announcementtypeid]').getValue() != chlk.models.announcement.AnnouncementTypeEnum.ANNOUNCEMENT.valueOf()){
                    var dropDown = this.dom.find('.new-item-dropdown');
                    if(!node.getValue() && !dropDown.is(':visible')){
                        this.dom.find('#list-last-button').triggerEvent('click');
                    }else{
                        if(event.type == 'keydown'){
                            var isUp = event.keyCode == ria.dom.Keys.UP.valueOf(),
                                isDown = event.keyCode == ria.dom.Keys.DOWN.valueOf();
                            if(dropDown.is(':visible') && (isUp || isDown)){
                                var current = dropDown.find('.current');
                                var currentIndex = parseInt(current.getData('index'),10), nextItem;
                                var moveUp = currentIndex > 0 && isUp;
                                var moveDown = currentIndex < (dropDown.getData('length') - 1) && isDown;
                                if(moveUp || moveDown){
                                    current.removeClass('current');
                                    if(moveUp){
                                        currentIndex--;
                                    }else{
                                        currentIndex++;
                                    }
                                    nextItem = dropDown.find('.autofill-item[data-index="' + currentIndex + '"]').addClass('current');
                                    node.setValue(nextItem.getData('value'));
                                }
                            }else{
                                dropDown.addClass('x-hidden');
                                if(event.keyCode == ria.dom.Keys.ENTER.valueOf())
                                    return false;
                            }
                        }
                    }
                }
            },

            OVERRIDE, VOID, function onPartialRender_(model, msg_) {
                BASE(model, msg_);
                if(model.getClass() == chlk.models.announcement.LastMessages){
                    this.dom.find('#content').triggerEvent('focus');
                }
            },

            OVERRIDE, VOID, function onStart_() {
                BASE();
                var that = this;
                handler = function(event, node){
                    var target = new ria.dom.Dom(event.target);
                    if(!target.parent('.drop-down-container').exists() && target.getAttr('name') != 'content')
                        that.dom.find('.new-item-dropdown').addClass('x-hidden');
                };
                new ria.dom.Dom().on('click', handler);
            },

            OVERRIDE, VOID, function onStop_() {
                new ria.dom.Dom('#save-form-button').triggerEvent('click');
                new ria.dom.Dom().off('click', handler);
                BASE();
            }
        ]
    );
});