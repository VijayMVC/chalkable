REQUIRE('chlk.models.id.AnnouncementId');

NAMESPACE('chlk.models.people', function () {
    "use strict";

    /** @class chlk.models.people.UsersListSubmit*/
    CLASS(
        'UsersListSubmit', [
            Number, 'selectedIndex',
            Boolean, 'byLastName',
            String, 'filter',
            Number, 'start',
            Number, 'count',
            String, 'submitType',
            String, 'rolesId',
            chlk.models.id.ClassId, 'classId',
            chlk.models.id.AnnouncementId, 'announcementId',

            Boolean, function isScroll(){
                return this.getSubmitType() == 'scroll';
            }
        ]);
});
