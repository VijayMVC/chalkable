REQUIRE('chlk.services.BaseService');
REQUIRE('ria.async.Future');
REQUIRE('chlk.models.announcement.ClassAnnouncementType');
REQUIRE('chlk.models.announcement.BaseAnnouncementViewData');

NAMESPACE('chlk.services', function () {
    "use strict";

    /** @class chlk.services.AnnouncementTypeService */
    CLASS(
        'AnnouncementTypeService', EXTENDS(chlk.services.BaseService), [

            [[ArrayOf(chlk.models.id.ClassId)]],
            ria.async.Future, function list(classIds){
                return this.get('AnnouncementType/ListByClasses.json', ArrayOf(chlk.models.announcement.ClassAnnouncementType),{
                    classIds: this.arrayToCsv(classIds)
                });
            },

            [[chlk.models.id.ClassId, String, String, Number, Number, Boolean, Number]],
            ria.async.Future, function create(classId, description_, name_, highScoresToDrop_, lowScoresToDrop_, isSystem_, percentage_){
                return this.post('AnnouncementType/Create.json', chlk.models.announcement.ClassAnnouncementType,{
                    classId: classId.valueOf(),
                    description: description_,
                    name: name_,
                    highScoresToDrop: highScoresToDrop_,
                    lowScoresToDrop: lowScoresToDrop_,
                    isSystem: isSystem_,
                    percentage: percentage_
                });
            },

            [[chlk.models.id.ClassId, String, String, Number, Number, Boolean, Number, Number]],
            ria.async.Future, function update(classId, description_, name_, highScoresToDrop_, lowScoresToDrop_, isSystem_, percentage_, classAnnouncementTypeId_){
                return this.post('AnnouncementType/Update.json', chlk.models.announcement.ClassAnnouncementType,{
                    classAnnouncementTypeId: classAnnouncementTypeId_,
                    classId: classId.valueOf(),
                    description: description_,
                    name: name_,
                    highScoresToDrop: highScoresToDrop_,
                    lowScoresToDrop: lowScoresToDrop_,
                    isSystem: isSystem_,
                    percentage: percentage_
                });
            },

            [[Array, chlk.models.announcement.AnnouncementTypeEnum]],
            ria.async.Future, function deleteTypes(ids, announcementType_){
                var type = announcementType_ || this.getContext().getSession().get(ChlkSessionConstants.ANNOUNCEMENT_TYPE, chlk.models.announcement.AnnouncementTypeEnum.CLASS_ANNOUNCEMENT);
                return this.post('AnnouncementType/Delete.json', Boolean,{
                    classAnnouncementTypeIds: this.arrayToCsv(ids),
                    announcementType: type.valueOf()
                });
            },

            [[chlk.models.id.ClassId, chlk.models.id.ClassId, Array]],
            ria.async.Future, function copyTypes(fromClassId, toClassId, typeIds){
                return this.post('AnnouncementType/Copy.json', Boolean, {
                    fromClassId: fromClassId.valueOf(),
                    toClassId: toClassId.valueOf(),
                    classAnnouncementTypeIds: this.arrayToCsv(ids)
                })
            }
        ]);
});