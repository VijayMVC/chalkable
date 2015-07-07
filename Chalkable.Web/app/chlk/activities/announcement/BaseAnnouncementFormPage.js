REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.announcement.BaseAnnouncementFormTpl');
REQUIRE('chlk.templates.announcement.AnnouncementAppAttachments');
REQUIRE('chlk.templates.announcement.LastMessages');

NAMESPACE('chlk.activities.announcement', function () {
    "use strict";

    /** @class chlk.activities.announcement.BaseAnnouncementFormPage*/
    CLASS(
        'BaseAnnouncementFormPage', EXTENDS(chlk.activities.lib.TemplatePage), [

            function $(){
                BASE();
                this._handler = null;
            },

            [ria.mvc.DomEventBind('change', '#expiresdate')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function dueDateChange(node, event){
                var dt = chlk.models.common.ChlkSchoolYearDate.GET_SCHOOL_YEAR_SERVER_DATE(node.getValue());
                if(!dt.valueOf())
                    node.setValue('');
            },

            [ria.mvc.DomEventBind('click', 'form')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function formClick(node, event){

            },

            [ria.mvc.DomEventBind('submit', '.announcement-form>FORM')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function submitClick(node, event){

                var that = this;
                var maxScoreNode = this.dom.find('[name="maxscore"]');
                if(maxScoreNode.exists() && !maxScoreNode.getValue())
                    maxScoreNode.setValue('100');
                setTimeout(function(){
                    var submitType = node.getData('submit-type');
                    if(submitType == "submitOnEdit" || submitType == "submit"){
                        that.dom.find('#save-form-button').remove();
                        //that.addPartialRefreshLoader();
                        node.setData('submit-type', null);
                    }
                }, 10);

            },

            [ria.mvc.DomEventBind('click', '.add-loader-btn')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function addLoaderOnSubmitClick(node, event){
                //this.addPartialRefreshLoader();
                this.dom.find('#save-form-button').remove();
            },

            /*[ria.mvc.DomEventBind('click', '#content')],
             [[ria.dom.Dom, ria.dom.Event]],
             VOID, function focusContent(node, event){
             node.triggerEvent('focus');
             },*/
            [ria.mvc.DomEventBind('click', '.add-standards .title')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function addStandardsClick(node, event){
                jQuery(node.parent('.add-standards').find('.add-standards-btn').valueOf()).click();
            },

            [ria.mvc.DomEventBind('click', '.advanced-options-btn')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function advancedOptionsButtonClick(node, event){
                var parentNode = node.parent('.advanced-options');
                node.toggleClass('selected');
                parentNode.find('.separator').toggleClass('x-hidden');
                parentNode.find('.advanced-options-container').toggleClass('x-hidden');
            },


            [ria.mvc.DomEventBind('click', '.drawer-icon')],
            [[ria.dom.Dom, ria.dom.Event]],
            function drawerIconClick(node, event){
                var dropDown = this.dom.find('.drop-down-container');
                if(!dropDown.is(':visible')){
                    this.dom.find('#list-last-button').trigger('click');
                    dropDown.show();
                }else{
                    this.dom.find('.new-item-dropdown').hide();
                    dropDown.hide();
                }
            },

            [ria.mvc.DomEventBind('mouseover', '.autofill-item')],
            [[ria.dom.Dom, ria.dom.Event]],
            function autoFillClick(node, event){
                if(!node.hasClass('current')){
                    node.parent().find('.current').removeClass('current');
                    node.addClass('current');
                }
            },

            [ria.mvc.DomEventBind('click', '.autofill-item')],
            [[ria.dom.Dom, ria.dom.Event]],
            function autoFillMouseOver(node, event){
                this.dom.find('textarea[name=content]').setValue(node.getData('value'));
                this.dom.find('.new-item-dropdown').hide();
                this.dom.find('.drop-down-container').hide();
            },

            [ria.mvc.DomEventBind('keydown', '#content')],
            [[ria.dom.Dom, ria.dom.Event]],
            function showDropDown(node, event){
                if(this.dom.find('[name=announcementtypeid]').getValue() != chlk.models.announcement.AnnouncementTypeEnum.ANNOUNCEMENT.valueOf()){
                    var dropDown = this.dom.find('.new-item-dropdown');
                    var isUp = event.keyCode == ria.dom.Keys.UP.valueOf(),
                        isDown = event.keyCode == ria.dom.Keys.DOWN.valueOf();
                    if(dropDown.is(':visible') && (isUp || isDown || event.keyCode == ria.dom.Keys.ENTER.valueOf())){
                        if(isUp || isDown){
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
                            }

                        }else{
                            node.setValue(dropDown.find('.current').getData('value'));
                            dropDown.hide();
                        }
                        return false;
                    }else{
                        dropDown.hide();
                    }
                }
                return true;
            },

            OVERRIDE, VOID, function onStart_() {
                BASE();
                var that = this;
                this._handler = function(node, event){
                    var target = new ria.dom.Dom(event.target);
                    if(!target.parent('.drop-down-container').exists() && target.getAttr('name') != 'content' && !target.is('.drawer-icon') && !target.is('#list-last-button')){
                        that.dom.find('.new-item-dropdown').hide();
                        that.dom.find('.drop-down-container').hide();
                    }

                };
                new ria.dom.Dom().on('click.dropdown', this._handler);
            },

            Boolean, 'notSave',

            [ria.mvc.DomEventBind('click', '.announcement-type-button')],
            [[ria.dom.Dom, ria.dom.Event]],
            function onTypeClick(node, event){
                if(node.hasClass('no-save'))
                    this.setNotSave(true);
            },

            OVERRIDE, VOID, function onStop_() {
                var button = new ria.dom.Dom('#save-form-button');
                if(button.exists() && !this.isNotSave())
                    button.trigger('click');
                this.setNotSave(false);
                new ria.dom.Dom().off('click.dropdown', this._handler);
                BASE();
            }
        ]
    );
});