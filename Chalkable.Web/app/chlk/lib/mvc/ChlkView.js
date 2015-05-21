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

            [[ImplementerOf(ria.mvc.IActivity)]],
            OVERRIDE, ria.mvc.IActivity, function get_(activityClass) {
                return this.prepareActivity_(BASE(activityClass));
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
            }
        ]);
});
