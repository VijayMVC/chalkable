REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.attendance.ClassList');
REQUIRE('chlk.templates.common.InfoMsg');

NAMESPACE('chlk.activities.attendance', function () {
    "use strict";

    /** @class chlk.activities.attendance.BasePostAttendancePage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.attendance.ClassList)],
        'BasePostAttendancePage', EXTENDS(chlk.activities.lib.TemplatePage), [


            function $(){
                BASE();
                this._needPopUp = false;
                this._submitFormSelector = '.set-attendance-list-form';
            },

            function datePickerHandler(event){
                var target = new ria.dom.Dom(event.target);
                if(target.is('.ui-state-default') || (target.is('td') && target.find('>.ui-state-default').exists()))
                    if(!this.tryToLeave(target)){
                        event.preventDefault();
                        event.stopPropagation();
                        return false;
                    }
            },

            [[ria.dom.Dom, Object]],
            function showLeavePopUp(node, needChange_){
                var buttons = [{
                    text: Msg.Leave_anyways,
                    color: chlk.models.common.ButtonColor.RED.valueOf(),
                    attributes: {
                        clazz: 'leave-button'
                    }
                }, {
                    text: Msg.Go_Back,
                    color: chlk.models.common.ButtonColor.GREEN.valueOf(),
                    attributes: {
                        clazz: 'go-back'
                    }
                }];
                var buttonsModel = [];
                var serializer = new chlk.lib.serialize.ChlkJsonSerializer();
                buttons.forEach(function(item){
                    buttonsModel.push(serializer.deserialize(item, chlk.models.common.Button));
                });
                var model = new chlk.models.common.InfoMsg(Msg.Click_post_to_save, Msg.Whoa.capitalize(), buttonsModel, 'attendance-leave');
                var tpl = new chlk.templates.common.InfoMsg();
                tpl.assign(model);
                tpl.renderTo(new ria.dom.Dom('#chlk-dialogs'));
                new ria.dom.Dom('#chlk-overlay, #chlk-dialogs').removeClass('x-hidden');
                var that = this;

                new ria.dom.Dom('.leave-button').on('click', function(){
                    that._needPopUp = false;
                    that.removeLeavePopUp();
                    node.trigger('click');
                });
                new ria.dom.Dom('.go-back').on('click', function(){
                    that.removeLeavePopUp();
                });
            },

            function removeLeavePopUp(){
                new ria.dom.Dom('#chlk-overlay, #chlk-dialogs').addClass('x-hidden');
                new ria.dom.Dom('#chlk-dialogs').setHTML('');
            },

            OVERRIDE, VOID, function onRender_(model){
                BASE(model);
                this._needPopUp = false;
                var that = this;
                new ria.dom.Dom('.page').on('click.leave', '.action-link:not(.class-button)', function(node, event){
                    return that.tryToLeave(node);
                });

                this.dom.find('.class-button:not(.pressed)').on('click.leave', function(node, event){
                    return that.tryToLeave(node);
                });

                document.addEventListener('click', this.datePickerHandler, true);
            },

            OVERRIDE, VOID, function onStop_() {
                BASE();
                new ria.dom.Dom('.page').off('click.leave', '.action-link:not(.class-button)');
                this.dom.find('.class-button:not(.pressed)').off('click.leave');
                document.removeEventListener('click', this.datePickerHandler, true);
            },

            [[ria.dom.Dom, Boolean]],
            function tryToLeave(node, needChange_){
                if(this._needPopUp && !node.parent(this._submitFormSelector).exists()){
                    this.showLeavePopUp(node, needChange_);
                    return false;
                }
                return true;
            }
        ])
});