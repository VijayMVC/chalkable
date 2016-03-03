REQUIRE('chlk.models.standard.Standard');
REQUIRE('chlk.models.id.ClassId');
REQUIRE('chlk.models.id.AnnouncementId');

NAMESPACE('chlk.models.standard', function () {
    "use strict";
    var SJX = ria.serialize.SJX;

    /** @class chlk.models.standard.StandardTableItem*/
    CLASS('StandardTableItem', EXTENDS(chlk.models.standard.Standard), [

        Boolean, 'selected',
        Number, 'row',
        Number, 'column',

        OVERRIDE, VOID, function deserialize(data) {
            BASE(data);
            this.row = SJX.fromValue(data.row, Number);
            this.column = SJX.fromValue(data.column, Number);
            this.selected = SJX.fromValue(data.isselected, Boolean);
        },

    ]);

    /** @class chlk.models.standard.StandardsTable*/
    CLASS('StandardsTable',IMPLEMENTS(ria.serialize.IDeserializable), [

        ArrayOf(ArrayOf(chlk.models.standard.StandardTableItem)), 'standardsColumns',

        [[ArrayOf(chlk.models.standard.StandardTableItem)]],
        VOID, function addColumn(standards){
            if(standards){
                var standardsColumns = this.getStandardsColumns();
                if(!standardsColumns){
                    standardsColumns = [];
                }
                standardsColumns.push(standards);
                this.setStandardsColumns(standardsColumns);
            }
        },

        [[ArrayOf(chlk.models.standard.StandardTableItem)]],
        function $createOneColumnTable(standardsColumn){
            BASE();
            this.addColumn(standardsColumn);
        },

        VOID, function deserialize(data){
            if(data && data.standardscolumns){
                this.standardsColumns = [];
                for(var i = 0; i < data.standardscolumns.length; i++){
                    if(data.standardscolumns[i] && data.standardscolumns[i].length > 0)
                        this.standardsColumns.push(SJX.fromArrayOfDeserializables(data.standardscolumns[i], chlk.models.standard.StandardTableItem));
                }
            }
        }
    ]);

    /** @class chlk.models.standard.StandardsTableViewData*/
    CLASS('StandardsTableViewData',[

        chlk.models.id.ClassId, 'classId',
        String, 'description',
        chlk.models.id.StandardSubjectId, 'subjectId',
        chlk.models.id.AnnouncementId, 'announcementId',

        chlk.models.standard.StandardsTable, 'standardsTable',

        [[String, chlk.models.id.ClassId, chlk.models.id.StandardSubjectId, chlk.models.standard.StandardsTable, chlk.models.id.AnnouncementId]],
        function $(description_, classId_, subjectId_, standardsTable_, announcementId_){
            BASE();
            if(standardsTable_)
                this.setStandardsTable(standardsTable_);
            if(classId_)
                this.setClassId(classId_);
            if(subjectId_)
                this.setSubjectId(subjectId_);
            if(description_)
                this.setDescription(description_);
            if(announcementId_)
                this.setAnnouncementId(announcementId_);
        }

    ]);
});
