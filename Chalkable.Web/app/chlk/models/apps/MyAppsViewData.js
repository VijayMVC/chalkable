REQUIRE('chlk.models.common.PaginatedList');

NAMESPACE('chlk.models.apps', function () {
    "use strict";
    /** @class chlk.models.apps.MyAppsViewData*/
    CLASS(
        'MyAppsViewData', [
            chlk.models.common.PaginatedList, 'apps',
            chlk.models.id.SchoolPersonId, 'personId',
            Boolean, 'editable',

            [[chlk.models.common.PaginatedList, Boolean, chlk.models.id.SchoolPersonId]],
            function $(apps, editable, personId){
                BASE();
                this.setApps(apps);
                this.setPersonId(personId);
                this.setEditable(editable);
            }
        ]);


});
