REQUIRE('ria.serialize.SJX');
REQUIRE('ria.serialize.IDeserializable');
REQUIRE('chlk.models.id.AnnouncementAssignedAttributeId');
REQUIRE('chlk.models.id.AnnouncementAttributeTypeId');

NAMESPACE('chlk.models.announcement', function(){


    var SJX = ria.serialize.SJX;

    /**@class chlk.models.announcement.AnnouncementAttributeViewData*/

    /*
     public int AnnouncementRef { get; set; }
     public int AttributeTypeId { get; set; }
     public string Uuid { get; set; }

     [NotDbFieldAttr]
     public int? SisActivityId { get; set; }
     [NotDbFieldAttr]
     public int? SisAttributeId { get; set; }
    * */




    UNSAFE, FINAL, CLASS('AnnouncementAttributeViewData', IMPLEMENTS(ria.serialize.IDeserializable), [
        chlk.models.id.AnnouncementAssignedAttributeId, 'id',

        String, 'name',

        String, 'text',

        String, 'uuid',

        Boolean, 'visibleForStudents',

        chlk.models.id.AnnouncementAttributeTypeId, 'attributeTypeId',


        //add attribute attachment

        VOID, function deserialize(raw) {
            this.id = SJX.fromValue(raw.id, chlk.models.id.AnnouncementAssignedAttributeId);
            this.name = SJX.fromValue(raw.name, String);
            this.text = SJX.fromValue(raw.text, String);
            this.uuid = SJX.fromValue(raw.uuid, String);
            this.visibleForStudents = SJX.fromValue(raw.visibleforstudents, Boolean);
            this.attributeTypeId = SJX.fromValue(raw.attributetypeid, chlk.models.id.AnnouncementAttributeTypeId);
        },


        Object, function getPostData(){
            return {
                name: this.getName() || '',
                text: this.getText() || '',
                uuid: this.getUuid() || '',
                visibleforstudents: this.isVisibleForStudents(),
                attributyTypeId: this.getAttributeTypeId().valueOf()
            }
        }
    ]);
});