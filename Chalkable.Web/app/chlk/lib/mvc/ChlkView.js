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
                return this.getContext().getSession().get('role');
            },

            chlk.models.people.User, function getCurrentUser_(){
                return this.getContext().getSession().get('currentPerson');
            },

            [[ria.mvc.IActivity]],
            ria.mvc.IActivity, function prepareActivity_(activity){
                //todo: ichlkactivity
                if (activity.setRole)
                    activity.setRole(this.getCurrentRole_());
                if (activity.setCurrentUser)
                    activity.setCurrentUser(this.getCurrentUser_());
                return activity;
            }
        ]);
});
