REQUIRE('chlk.models.id.AppId');
NAMESPACE('chlk.models.apps', function () {
    "use strict";
    /** @class chlk.models.apps.AppInstallPostData*/
    CLASS(
        'AppInstallPostData', [
            chlk.models.id.AppId, 'appId',
            String, 'classes',
            String, 'departments',
            String, 'gradeLevels',
            String, 'roles',
            String, 'submitActionType',
            String, 'forAll',
            String, 'groups',
            chlk.models.id.AppInstallGroupId, 'currentPerson',
            Boolean, 'fromNewItem',

            Boolean, function isInstallForAll(){
                return this.getForAll() == 'all';
            },

            Boolean, function isEmpty(){
                return this.getClasses() == '' && this.getDepartments() == '' &&
                    this.getGradeLevels() == '' && this.getRoles() == '' && this.getCurrentPerson().valueOf() == '' && this.getForAll() == '';
            }
        ]);
});
