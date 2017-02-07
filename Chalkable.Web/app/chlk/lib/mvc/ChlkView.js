REQUIRE('ria.mvc.View');
REQUIRE('chlk.models.common.Role');
REQUIRE('chlk.models.people.User');

NAMESPACE('chlk.lib.mvc', function () {
    "use strict";

    /**
     * @class chlk.lib.mvc.ChlkView
     */
    CLASS(
        'ChlkView', EXTENDS(ria.mvc.View), [

            [[ImplementerOf(ria.mvc.IActivity), ria.mvc.ActivityViewMode]],
            OVERRIDE, ria.mvc.IActivity, function get_(activityClass, viewMode) {
                return this.prepareActivity_(BASE(activityClass, viewMode));
            },

            chlk.models.common.Role, function getCurrentRole_(){
                return this.getContext().getSession().get(ChlkSessionConstants.USER_ROLE);
            },

            chlk.models.people.User, function getCurrentUser_(){
                return this.getContext().getSession().get(ChlkSessionConstants.CURRENT_PERSON);
            },

            Boolean, function isStudyCenterEnabled_(){
                return this.getContext().getSession().get(ChlkSessionConstants.STUDY_CENTER_ENABLED);
            },


            Boolean, function isLEIntegrated_(){
                var leParams = this.getContext().getSession().get(ChlkSessionConstants.LE_PARAMS, new chlk.models.school.LEParams());
                return leParams.isLEIntegrated();
            },

            Boolean, function isAssessmentEnabled_(){
                return this.getContext().getSession().get(ChlkSessionConstants.ASSESSMENT_ENABLED);
            },

            Boolean, function isStackEmpty(){
                var stackLen = this.getStack_().length;
                return stackLen == 0 || stackLen == 1 && !this.getCurrent().isRendered();
            },

            ria.async.Future, function reset() {
                return this.reset_();
            },

            [[String]],
            function submitToIFrame(src){
                new ria.dom.Dom('.report-iframe').remove();
                var iframe = new ria.dom.Dom('<iframe class="report-iframe">');
                iframe.setAttr('src', src);
                iframe.appendTo('body');
            },

            [[ria.mvc.IActivity]],
            ria.mvc.IActivity, function prepareActivity_(activity){
                //todo: ichlkactivity
                if (activity.setRole)
                    activity.setRole(this.getCurrentRole_());
                if (activity.setCurrentUser)
                    activity.setCurrentUser(this.getCurrentUser_());
                if (activity.setStudyCenterEnabled)
                    activity.setStudyCenterEnabled(this.isStudyCenterEnabled_());
                if (activity.setLEIntegrated)
                    activity.setLEIntegrated(this.isLEIntegrated_());
                if (activity.setAssessmentEnabled)
                    activity.setAssessmentEnabled(this.isAssessmentEnabled_());
                return activity;
            },

            [[Number]],
            VOID, function setNewNotificationCount(count) {
                var $link = ria.dom.Dom('.notifications-link'),
                    $span = $link.find('span');

                if (count < 1) {
                    $span.removeSelf()
                } else if ($span.exists()) {
                    $span.setText(String(count));
                } else {
                    ria.dom.Dom.$fromHTML('<span>' + count + '</span>').appendTo($link);
                }

                var title = document.title;
                var pos = title.indexOf('(');
                if (pos > -1)
                    title = title.slice(0, pos - 1);

                document.title = title +(count ? ' (' + count + ')' : '');
            },

            [[String]],
            VOID, function updateUserName(userName) {
                ria.dom.Dom('header .logout-area').setText(userName);
            },

            [[String, String, Array, String, Boolean, String, Object]],
            chlk.models.common.InfoMsg, function getMessageBoxModel_(text_, header_, buttons_, clazz_, isHtmlText_, inputType_, inputValue_, inputAttrs_){
                var buttons = [];
                if(buttons_){
                    var serializer = new chlk.lib.serialize.ChlkJsonSerializer();
                    buttons_.forEach(function(item){
                        buttons.push(serializer.deserialize(item, chlk.models.common.Button));
                    });
                }else{
                    buttons.push(new chlk.models.common.Button('Ok'));
                }
                return new chlk.models.common.InfoMsg(text_, header_, buttons, clazz_, isHtmlText_, inputType_, inputValue_, inputAttrs_);
            },

            [[String, String, Array, String, Boolean]],
            ria.async.Future, function ShowMsgBox(text_, header_, buttons_, clazz_, isHtmlText_, inputType_, inputValue_, inputAttrs_) {
                var model = this.getMessageBoxModel_(text_, header_, buttons_, clazz_, isHtmlText_, inputType_, inputValue_, inputAttrs_);
                return this.showModal(chlk.activities.common.InfoMsgDialog, model);
            },

            ria.async.Future, function ShowConfirmBox(text, clazz_) {
                return this.ShowMsgBox(text, null,
                    [{text: 'Cancel'}, {text: 'OK', clazz: 'blue-button', value: 'ok'}], clazz_ || 'leave-msg', true)
                    .then(function (mrResult) {
                        if (!mrResult)
                            return ria.async.BREAK;

                        return mrResult;
                    });
            },

            ria.async.Future, function ShowLeaveConfirmBox() {
                return this.ShowConfirmBox("<b>Are you sure you want to leave this page?</b></br><span>You will lose unsaved changes.</span>", 'leave-msg')
                    .then(function (data) {
                        return data == 'ok';
                    });
            }
        ]);
});
