REQUIRE('chlk.models.admin.BaseStatistic');
REQUIRE('chlk.models.id.SchoolYearId');
REQUIRE('chlk.models.id.SchoolId');
REQUIRE('chlk.models.id.SchoolPersonId');
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

            chlk.models.id.GradingPeriodId, 'gradingPeriodId',

            String, 'schoolName',

            chlk.models.id.SchoolId, 'schoolId',

            Number, 'sortType',

            chlk.models.id.SchoolPersonId, 'teacherId',

            [[
                ArrayOf(chlk.models.admin.BaseStatistic),
                Number,
                chlk.models.id.SchoolId,
                String,
                String,
                chlk.models.id.SchoolPersonId,
                chlk.models.id.SchoolYearId
            ]],
            function $(items_, sortType_, schoolId_, schoolName_, filter_, teacherId_, schoolYearId_){
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
                if(teacherId_)
                    this.setTeacherId(teacherId_);
                if(schoolYearId_)
                    this.setSchoolYearId(schoolYearId_);
            },

            VOID, function deserialize(raw){
                this.sortType = SJX.fromValue(raw.sortType, Number);
                this.filter = SJX.fromValue(raw.filter, String);
                this.submitType = SJX.fromValue(raw.submitType, String);
                this.start = SJX.fromValue(raw.start, Number);
                this.schoolYearId = SJX.fromValue(raw.schoolYearId, chlk.models.id.SchoolYearId);
                this.gradingPeriodId = SJX.fromValue(raw.gradingPeriodId, chlk.models.id.GradingPeriodId);
                this.schoolName = SJX.fromValue(raw.schoolName, String);
                this.schoolId = SJX.fromValue(raw.schoolId, chlk.models.id.SchoolId);
                this.teacherId = SJX.fromValue(raw.teacherId, chlk.models.id.SchoolPersonId);
            },

            Boolean, function isNotEmptyStatistic(){
                return (this.getItems().filter(function(school){return school.getAvg() !== null}).length +
                    this.getItems().filter(function(school){return school.getInfractionsCount() !== null}).length +
                    this.getItems().filter(function(school){return school.getAbsences() !== null}).length) > 0
            }
        ]);
});
