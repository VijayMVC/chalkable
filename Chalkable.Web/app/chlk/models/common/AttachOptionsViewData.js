REQUIRE('chlk.models.id.SchoolPersonId');
REQUIRE('chlk.models.id.AnnouncementId');
REQUIRE('chlk.models.id.ClassId');
REQUIRE('chlk.models.id.AnnouncementAssignedAttributeId');

NAMESPACE('chlk.models.common', function () {

    var SJX = ria.serialize.SJX;

    "use strict";
    /** @class chlk.models.common.AttachOptionsViewData*/
    CLASS(
        'AttachOptionsViewData', IMPLEMENTS(ria.serialize.IDeserializable), [

            chlk.models.id.AnnouncementId, 'announcementId',
            chlk.models.id.ClassId, 'classId',
            chlk.models.id.AppId, 'assessmentAppId',
            String, 'announcementTypeName',

            chlk.models.announcement.AnnouncementTypeEnum, 'announcementType',

            Boolean, 'assessmentAttachEnabled',
            Boolean, 'fileCabinetEnabled',
            Boolean, 'standardAttachEnabled',
            Boolean, 'ableAttachApps',
            chlk.models.id.AnnouncementAssignedAttributeId, 'assignedAttributeId',
            String, 'appUrlAppend',
            ArrayOf(chlk.models.apps.Application), 'externalAttachApps',
            Boolean, 'dialog',

            VOID, function deserialize(raw) {
                this.assessmentAppId = SJX.fromValue(raw.assessmentapplicationid, chlk.models.id.AppId);
                //console.log(raw.assessmentapplicationid);
                //console.log(this.assessmentAppId);

                if(this.assessmentAppId)
                    this.assessmentAttachEnabled = true;
                this.fileCabinetEnabled = SJX.fromValue(raw.isfilecabinetenabled, Boolean);
                this.standardAttachEnabled = SJX.fromValue(raw.isstandardenabled, Boolean);
                this.externalAttachApps = SJX.fromArrayOfDeserializables(raw.externalattachapps, chlk.models.apps.Application);
                this.ableAttachApps = SJX.fromValue(raw.isappsenabled, Boolean);
            },

            function updateByValues(standardAttachEnabled_, assessmentAttachEnabled_, announcementId_, classId_,
                                    announcementTypeName_, announcementType_, assignedAttributeId_, appUrlAppend_, ableAttachApps_, isDialog_){
                if(typeof standardAttachEnabled_ == 'boolean')
                    this.setStandardAttachEnabled(standardAttachEnabled_);
                if(typeof assessmentAttachEnabled_ == 'boolean')
                    this.setAssessmentAttachEnabled(assessmentAttachEnabled_);
                if(typeof ableAttachApps_ == 'boolean')
                    this.setAbleAttachApps(ableAttachApps_);
                if(announcementId_)
                    this.setAnnouncementId(announcementId_);
                if(classId_)
                    this.setClassId(classId_);
                if(announcementTypeName_)
                    this.setAnnouncementTypeName(announcementTypeName_);
                if(announcementType_)
                    this.setAnnouncementType(announcementType_);
                if(assignedAttributeId_)
                    this.setAssignedAttributeId(assignedAttributeId_);
                if(appUrlAppend_)
                    this.setAppUrlAppend(appUrlAppend_);
                if(isDialog_)
                    this.setDialog(isDialog_);
            }
        ]);
});
