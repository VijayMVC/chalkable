REQUIRE('chlk.models.id.ClassId');

NAMESPACE('chlk.models.attendance', function () {
    "use strict";

    /** @class chlk.models.attendance.EditSeatingGridViewData*/
    CLASS(
        'EditSeatingGridViewData', [
            Number, 'columns',

            Number, 'rows',

            chlk.models.id.ClassId, 'classId',

            function $(classId_, rows_, columns_){
                BASE();
                if(classId_)
                    this.setClassId(classId_);
                if(rows_ || rows_ == 0)
                    this.setRows(rows_);
                if(columns_ || columns_ == 0)
                    this.setColumns(columns_);
            }
        ]);
});
