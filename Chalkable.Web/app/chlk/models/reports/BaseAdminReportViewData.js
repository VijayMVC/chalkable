REQUIRE('ria.serialize.SJX');

NAMESPACE('chlk.models.reports', function () {
    "use strict";

    /** @class chlk.reports.AdminReportTypeEnum*/
    ENUM('AdminReportTypeEnum', {
        LUNCH_COUNT: 1,
        REPORT_CARD: 2
    });

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.reports.BaseAdminReportViewData*/
    CLASS('BaseAdminReportViewData', IMPLEMENTS(ria.serialize.IDeserializable), [
        String, 'title',
        String, 'groupIds',
        String, 'studentIds',
        String, 'submitType',
        Boolean, 'ableDownload',
        String, 'selectedItems',
        String, 'recipientsToAdd',
        chlk.models.reports.AdminReportTypeEnum, 'reportType',
        chlk.models.reports.StudentIdentifierEnum, 'idToPrint',

        function getParsedSelected(){
            var selectedItems = this.selectedItems ? JSON.parse(this.selectedItems ) : {groups:[], students:[]};

            selectedItems.groups = selectedItems.groups.map(function(group){
                return new chlk.models.group.Group(group.name, new chlk.models.id.GroupId(group.id));
            });

            selectedItems.students = selectedItems.students.map(function(student){
                return new chlk.models.people.ShortUserInfo(null, null, new chlk.models.id.SchoolPersonId(student.id), student.displayname, student.gender);
            });

            return selectedItems;
        },

        VOID, function deserialize(raw) {
            this.title = SJX.fromValue(raw.title, String);
            this.groupIds = SJX.fromValue(raw.groupIds, String);
            this.studentIds = SJX.fromValue(raw.studentIds, String);
            this.submitType = SJX.fromValue(raw.submitType, String);
            this.idToPrint = SJX.fromValue(raw.idToPrint, chlk.models.reports.StudentIdentifierEnum);
            this.ableDownload = SJX.fromValue(raw.ableDownload, Boolean);
            this.selectedItems = SJX.fromValue(raw.selectedItems, String);
            this.recipientsToAdd = SJX.fromValue(raw.recipientsToAdd, String);
            this.reportType = SJX.fromValue(parseInt(raw.reportType, 10), chlk.models.reports.AdminReportTypeEnum);
        },

        [[Boolean, Number]],
        function $(ableDownload_, defaultIdToPrint_){
            BASE();
            if(ableDownload_)
                this.setAbleDownload(ableDownload_);
            if(defaultIdToPrint_)
                this.setIdToPrint(chlk.models.reports.StudentIdentifierEnum(defaultIdToPrint_ + 1));
            else this.setIdToPrint(chlk.models.reports.StudentIdentifierEnum.NONE);
        }
    ]);
});
