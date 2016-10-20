REQUIRE('ria.serialize.SJX');
REQUIRE('chlk.models.common.ChlkDate');

NAMESPACE('chlk.models.reports', function () {
    "use strict";

    /** @class chlk.models.reports.LunchCountOrderBy*/
    ENUM('LunchCountOrderBy',{
        BY_CLASS: 0,
        MEAL_TYPE: 1,
        STUDENT: 2
    });

    /** @class chlk.models.reports.LunchCountAdditionalOptions*/
    ENUM('LunchCountAdditionalOptions',{
        GROUP_TOTALS: 0,
        GRAND_TOTALS: 1,
        STUDENTS_ONLY: 2,
        SUMMARY_ONLY: 3
    });


    var SJX = ria.serialize.SJX;

    /** @class chlk.models.reports.SubmitLunchCountViewData*/

    CLASS('SubmitLunchCountViewData', IMPLEMENTS(ria.serialize.IDeserializable),  [
        String, 'title',
        String, 'groupIds',
        String, 'studentIds',
        String, 'submitType',
        String, 'includeOptions',
        Boolean, 'allActiveMeals',
        chlk.models.reports.LunchCountOrderBy, 'orderBy',
        chlk.models.reports.StudentIdentifierEnum, 'idToPrint',
        chlk.models.common.ChlkDate, 'startDate',
        chlk.models.common.ChlkDate, 'endDate',
        Boolean, 'ableDownload',
        String, 'selectedItems',
        String, 'recipientsToAdd',
        
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
            this.includeOptions = SJX.fromValue(raw.includeOptions, String);
            this.allActiveMeals = SJX.fromValue(raw.allActiveMeals, Boolean);
            this.orderBy = SJX.fromValue(raw.orderBy, chlk.models.reports.LunchCountOrderBy);
            this.idToPrint = SJX.fromValue(raw.idToPrint, chlk.models.reports.StudentIdentifierEnum);
            this.startDate = SJX.fromDeserializable(raw.startDate, chlk.models.common.ChlkDate);
            this.endDate = SJX.fromDeserializable(raw.endDate, chlk.models.common.ChlkDate);
            this.ableDownload = SJX.fromValue(raw.ableDownload, Boolean);
            this.selectedItems = SJX.fromValue(raw.selectedItems, String);
            this.recipientsToAdd = SJX.fromValue(raw.recipientsToAdd, String);
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
