REQUIRE('chlk.models.common.ChlkDate');
REQUIRE('chlk.models.people.ShortUserInfo');
REQUIRE('chlk.models.id.DepartmentId');

NAMESPACE('chlk.models.search', function () {

    "use strict";
    /** @class chlk.models.search.SearchItem*/
    CLASS(
        'SearchItem', [
            String, 'id',
            String, 'description',

            [ria.serialize.SerializeProperty('shortpersoninfo')],
            chlk.models.people.ShortUserInfo, 'personInfo',

            [ria.serialize.SerializeProperty('searchtype')],
            Number, 'searchType',
            [ria.serialize.SerializeProperty('announcementid')],
            String, 'announcementId',
            [ria.serialize.SerializeProperty('announcementtype')],
            Number, 'announcementType',
            [ria.serialize.SerializeProperty('isadminannouncement')],
            Boolean, 'adminAnnouncement',
            [ria.serialize.SerializeProperty('smallpictureid')],
            String, 'smallPictureId',
            [ria.serialize.SerializeProperty('departmentid')],
            chlk.models.id.DepartmentId, 'departmentId',

            READONLY, Number, 'chalkableAnnouncementType',
            Number, function getChalkableAnnouncementType(){
                var res = this.getAnnouncementType();
                if(res) return res;
                if(this.isAdminAnnouncement()) return chlk.models.announcement.AnnouncementTypeEnum.ADMIN.valueOf();
                return chlk.models.announcement.AnnouncementTypeEnum.ANNOUNCEMENT.valueOf();
            }
        ]);
});

