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

            Boolean, 'fileCabinetEnabled',
            Boolean, 'standardAttachEnabled',
            Boolean, 'showApps',
            chlk.models.id.AnnouncementAssignedAttributeId, 'assignedAttributeId',
            String, 'appUrlAppend',
            ArrayOf(chlk.models.apps.Application), 'externalAttachApps',

            VOID, function deserialize(raw) {
                this.assessmentAppId = SJX.fromValue(raw.assessmentapplicationid, chlk.models.id.AppId);
                this.fileCabinetEnabled = SJX.fromValue(raw.isfilecabinetenabled, Boolean);
                this.standardAttachEnabled = SJX.fromValue(raw.isstandardenabled, Boolean);
                this.showApps = SJX.fromValue(raw.isappsenabled, Boolean);
                this.externalAttachApps = SJX.fromArrayOfDeserializables(raw.externalattachapps, chlk.models.apps.Application);
            },

            function updateByValues(standardAttachEnabled_, showApps_, announcementId_, classId_, announcementTypeName_, announcementType_, assignedAttributeId_, appUrlAppend_){
                if(typeof standardAttachEnabled_ == 'boolean')
                    this.setStandardAttachEnabled(standardAttachEnabled_);
                if(typeof showApps_ == 'boolean')
                    this.setShowApps(showApps_);
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
            }
        ]);
});
