REQUIRE('chlk.models.common.ChlkDate');
REQUIRE('chlk.models.course.Course');


NAMESPACE('chlk.models.search', function () {

    "use strict";
    /** @class chlk.models.search.SearchItem*/
    CLASS(
        'SearchItem', [
            String, 'id',
            String, 'description',
            String, 'gender',
            [ria.serialize.SerializeProperty('searchtype')],
            Number, 'searchType',
            [ria.serialize.SerializeProperty('roleid')],
            Number, 'roleId',
            chlk.models.course.Course, 'course',
            [ria.serialize.SerializeProperty('announcementid')],
            String, 'announcementId',
            [ria.serialize.SerializeProperty('announcementtype')],
            Number, 'announcementType',
            [ria.serialize.SerializeProperty('isadminannouncement')],
            Number, 'adminAnnouncement',

            READONLY, Number, 'chalkableAnnouncementType',
            Number, function getChalkableAnnouncementType(){
                var res = this.getAnnouncementType();
                if(res) return res;
                if(this.isAdminAnnouncement()) return chlk.models.announcement.AnnouncementTypeEnum.ADMIN.valueOf();
                return chlk.models.announcement.AnnouncementTypeEnum.ANNOUNCEMENT.valueOf();
            }
        ]);
});

