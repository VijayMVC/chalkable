NAMESPACE('chlk.models.apps', function () {
    "use strict";
    /** @class chlk.models.apps.AppAccess*/
    CLASS(
        'AppAccess', [
            [ria.serialize.SerializeProperty('hasstudentmyapps')],
            Boolean, 'studentMyAppsEnabled',
            [ria.serialize.SerializeProperty('hasteachermyapps')],
            Boolean, 'teacherMyAppsEnabled',
            [ria.serialize.SerializeProperty('hasadminmyapps')],
            Boolean, 'adminMyAppsEnabled',
            [ria.serialize.SerializeProperty('hasparentmyapps')],
            Boolean, 'parentMyAppsEnabled',
            [ria.serialize.SerializeProperty('hasmypappsview')],
            Boolean, 'myAppsViewVisible',
            [ria.serialize.SerializeProperty('canattach')],
            Boolean, 'attachEnabled',
            [ria.serialize.SerializeProperty('showingradeview')],
            Boolean, 'visibleInGradingView',

            Object, function getPostData(){
                return {
                    hasstudentmyapps: this.isStudentMyAppsEnabled(),
                    hasteachermyapps: this.isTeacherMyAppsEnabled(),
                    hasadminmyapps: this.isAdminMyAppsEnabled(),
                    hasparentmyapps: this.isParentMyAppsEnabled(),
                    canattach: this.isAttachEnabled(),
                    showingradeview: this.isVisibleInGradingView()
                }
            },

            [[Boolean, Boolean, Boolean, Boolean, Boolean, Boolean]],
            function $(hasStudentMyApps_, hasTeacherMyApps_, hasAdminMyApps_, hasParentMyApps_, canAttach_, showInGradeView_){
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
            }
        ]);
});
