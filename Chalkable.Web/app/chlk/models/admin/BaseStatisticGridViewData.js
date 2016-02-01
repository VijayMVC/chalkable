REQUIRE('chlk.models.admin.BaseStatistic');
REQUIRE('chlk.models.id.SchoolYearId');
REQUIRE('chlk.models.id.SchoolId');
REQUIRE('ria.serialize.SJX');

NAMESPACE('chlk.models.admin', function () {
    "use strict";

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.admin.BaseStatisticGridViewData*/
    CLASS(
        'BaseStatisticGridViewData', IMPLEMENTS(ria.serialize.IDeserializable), [
            ArrayOf(chlk.models.admin.BaseStatistic), 'items',

            String, 'filter',

            String, 'submitType',

            Number, 'start',

            chlk.models.id.SchoolYearId, 'schoolYearId',

            String, 'schoolName',

            chlk.models.id.SchoolId, 'schoolId',

            Number, 'sortType',

            function $(items_, sortType_, schoolId_, schoolName_, filter_){
                BASE();
                if(items_)
                    this.setItems(items_);
                if(filter_)
                    this.setFilter(filter_);
                if(schoolId_)
                    this.setSchoolId(schoolId_);
                if(schoolName_)
                    this.setSchoolName(schoolName_);
                if(sortType_ || sortType_ == 0)
                    this.setSortType(sortType_);
            },

            VOID, function deserialize(raw){
                this.sortType = SJX.fromValue(raw.sortType, Number);
                this.filter = SJX.fromValue(raw.filter, String);
                this.submitType = SJX.fromValue(raw.submitType, String);
                this.start = SJX.fromValue(raw.start, Number);
                this.schoolYearId = SJX.fromValue(raw.schoolYearId, chlk.models.id.SchoolYearId);
                this.schoolName = SJX.fromValue(raw.schoolName, String);
                this.schoolId = SJX.fromValue(raw.schoolId, chlk.models.id.SchoolId);
            }
        ]);
});
