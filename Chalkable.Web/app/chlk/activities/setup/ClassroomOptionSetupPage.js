REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.setup.ClassroomOptionSetupTpl');

NAMESPACE('chlk.activities.setup', function () {

    /** @class chlk.activities.setup.ClassroomOptionSetupPage*/
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.setup.ClassroomOptionSetupTpl)],
        'ClassroomOptionSetupPage', EXTENDS(chlk.activities.lib.TemplatePage), [
            function $() {
                BASE();
                this._needPopUp = true;
            },

            OVERRIDE, VOID, function onRender_(model) {
                BASE(model);
                this.dom.find('.submit-form').setData('values', jQuery('.submit-form').serialize());
                var that = this;
                new ria.dom.Dom('#page').on('click.leave', '.action-link:not(.pressed)', function(node, event){
                    if(that.dom.find('.submit-form').exists())
                        return that.tryToLeave(node);

                    return true;
                });
            },

            OVERRIDE, VOID, function onStop_() {
                BASE();
                new ria.dom.Dom('#page').off('click.leave', '.action-link:not(.pressed)');
            },

            OVERRIDE, VOID, function onPartialRender_(model, msg_) {
                BASE(model, msg_);
                this._needPopUp = true;
                this.dom.find('.submit-form').setData('values', jQuery('.submit-form').serialize());
            },

            [[ria.dom.Dom]],
            function tryToLeave(node){
                if(!this._needPopUp || this.dom.find('.submit-form').getData('values') == jQuery('.submit-form').serialize().replace(/(false|true)/g, '0'))
                    return true;

                this.showLeavePopUp(node);
                return false;
            },

            [[ria.dom.Dom, Boolean]],
            function showLeavePopUp(node){
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
                var model = new chlk.models.common.InfoMsg('This screen contains unsaved information. \n Are you sure you want to navigate away?', Msg.Whoa.capitalize(), buttonsModel);
                var tpl = new chlk.templates.common.InfoMsg();
                tpl.assign(model);
                new ria.dom.Dom('#chlk-dialogs').appendChild('<div class="info-msg-dialog"></div>');
                tpl.renderTo(new ria.dom.Dom('.info-msg-dialog'));
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
                new ria.dom.Dom('#chlk-dialogs').find('.info-msg-dialog').remove();
            }
        ]);
});