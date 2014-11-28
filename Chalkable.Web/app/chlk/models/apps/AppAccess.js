REQUIRE('ria.serialize.SJX');
REQUIRE('ria.serialize.IDeserializable');

NAMESPACE('chlk.models.apps', function () {
    "use strict";

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.apps.AppAccess*/
    CLASS(
        UNSAFE, FINAL,   'AppAccess',IMPLEMENTS(ria.serialize.IDeserializable), [
            VOID, function deserialize(raw){
                this.studentMyAppsEnabled = SJX.fromValue(raw.hasstudentmyapps, Boolean);
                this.teacherMyAppsEnabled = SJX.fromValue(raw.hasteachermyapps, Boolean);
                this.adminMyAppsEnabled = SJX.fromValue(raw.hasadminmyapps, Boolean);
                this.parentMyAppsEnabled = SJX.fromValue(raw.hasparentmyapps, Boolean);
                this.attachEnabled = SJX.fromValue(raw.canattach, Boolean);
                this.visibleInGradingView = SJX.fromValue(raw.showingradeview, Boolean);
                this.adjustedToStandards = SJX.fromValue(raw.adjustedtostandards, Boolean);
                this.myAppsForCurrentRoleEnabled = SJX.fromValue(raw.myappsforcurrentroleenabled, Boolean);
            },
            Boolean, 'studentMyAppsEnabled',
            Boolean, 'teacherMyAppsEnabled',
            Boolean, 'adminMyAppsEnabled',
            Boolean, 'parentMyAppsEnabled',
            Boolean, 'attachEnabled',
            Boolean, 'visibleInGradingView',
            Boolean, 'adjustedToStandards',
            Boolean, 'myAppsForCurrentRoleEnabled',

            Object, function getPostData(){
                return {
                    hasstudentmyapps: this.isStudentMyAppsEnabled(),
                    hasteachermyapps: this.isTeacherMyAppsEnabled(),
                    hasadminmyapps: this.isAdminMyAppsEnabled(),
                    hasparentmyapps: this.isParentMyAppsEnabled(),
                    canattach: this.isAttachEnabled(),
                    showingradeview: this.isVisibleInGradingView(),
                    adjustedtostandarts: this.isAdjustedToStandards()
                }
            },

            [[Boolean, Boolean, Boolean, Boolean, Boolean, Boolean, Boolean]],
            function $(hasStudentMyApps_, hasTeacherMyApps_, hasAdminMyApps_, hasParentMyApps_, canAttach_, showInGradeView_, adjustedToStandarts_){
                BASE();
                if (hasStudentMyApps_)
                    this.setStudentMyAppsEnabled(hasStudentMyApps_);
                if (hasTeacherMyApps_)
                    this.setTeacherMyAppsEnabled(hasTeacherMyApps_);
                if (hasAdminMyApps_)
                    this.setAdminMyAppsEnabled(hasAdminMyApps_);
                if (hasParentMyApps_)
                    this.setParentMyAppsEnabled(hasParentMyApps_);
                if (canAttach_)
                    this.setAttachEnabled(canAttach_);
                if (showInGradeView_)
                    this.setVisibleInGradingView(showInGradeView_);
                if (adjustedToStandarts_)
                    this.setAdjustedToStandarts(adjustedToStandarts_);
            }

        ]);
});
