REQUIRE('chlk.models.announcement.AnnouncementType');
REQUIRE('chlk.models.id.ClassId');

NAMESPACE('chlk.models.announcement', function () {
    "use strict";

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.announcement.ClassAnnouncementType*/
    CLASS(
        UNSAFE, 'ClassAnnouncementType', EXTENDS(chlk.models.announcement.AnnouncementType), IMPLEMENTS(ria.serialize.IDeserializable), [
            [[Number, String, chlk.models.id.ClassId, Boolean]],
            function $(id_, name_, classId_, ableEdit_){
                BASE();
                if(id_)
                    this.setId(id_);
                if(name_)
                    this.setName(name_);
                if(classId_)
                    this.setClassId(classId_);
                if(ableEdit_)
                    this.setAbleEdit(ableEdit_);
            },

            OVERRIDE, VOID, function deserialize(raw){
                BASE(raw);
                this.id = SJX.fromValue(raw.id, Number);
                this.ids = SJX.fromValue(raw.ids, String);
                this.classId = SJX.fromValue(raw.classid, chlk.models.id.ClassId);
                this.highScoresToDrop = SJX.fromValue(raw.highscorestodrop, Number);
                this.lowScoresToDrop = SJX.fromValue(raw.lowscorestodrop, Number);
                this.percentage = SJX.fromValue(raw.percentage, Number);
                this.ableEdit = SJX.fromValue(raw.ableEdit, Boolean);
            },

            Number, 'id',

            String, 'ids',

            chlk.models.id.ClassId, 'classId',

            Number, 'highScoresToDrop',

            Number, 'lowScoresToDrop',

            Number, 'percentage',

            Boolean, 'ableEdit'
        ]);
});
