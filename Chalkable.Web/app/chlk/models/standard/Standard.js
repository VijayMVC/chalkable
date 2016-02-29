REQUIRE('ria.serialize.SJX');
REQUIRE('ria.serialize.IDeserializable');
REQUIRE('chlk.models.id.StandardId');
REQUIRE('chlk.models.id.AnnouncementId');
REQUIRE('chlk.models.id.StandardSubjectId');

NAMESPACE('chlk.models.standard', function () {
    "use strict";

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.standard.Standard*/
    CLASS(UNSAFE,
        'Standard', IMPLEMENTS(ria.serialize.IDeserializable),  [
            VOID, function deserialize(raw) {
                this.name = SJX.fromValue(raw.name, String);
                this.description = SJX.fromValue(raw.description, String);
                this.announcementId = SJX.fromValue(raw.announcementid, chlk.models.id.AnnouncementId);
                this.standardId = SJX.fromValue(raw.standardid, chlk.models.id.StandardId);
                this.grade = SJX.fromValue(raw.grade, String);
                this.commonCoreStandardCode = SJX.fromArrayOfValues(raw.ccstandardcodes, String);
                this.academicBenchmarkId = SJX.fromValue(raw.academicbenchmarkid, String);
                this.subjectId = SJX.fromValue(raw.standardsubjectid, chlk.models.id.StandardSubjectId);
            },

            String, 'academicBenchmarkId',
            String, 'name',
            String, 'description',
            chlk.models.id.AnnouncementId, 'announcementId',
            chlk.models.id.StandardId, 'standardId',
            String, 'grade',
            String, 'commonCoreStandardCode',
            chlk.models.id.StandardSubjectId, 'subjectId',

            function getCommonCoreStandardCode(){
                return this.commonCoreStandardCode && this.commonCoreStandardCode[0];
            },

            function getCommonCoreStandardCodes(){
                return this.commonCoreStandardCode && this.commonCoreStandardCode.join(',');
            },

            function getCommonCoreStandardCodesArray(){
                return this.commonCoreStandardCode;
            },

            String, function displayTitle(){
                var name = this.getName();
                if(name && (!name.trim || name.trim() != '')) return name;
                return this.getDescription();
            },

            [[chlk.models.id.StandardId, String, String, String]],
            function $(standardId_, name_, grade_, ccStandardCode_){
                BASE();
                if(standardId_)
                    this.setStandardId(standardId_);
                if(name_)
                    this.setName(name_);
                if(grade_)
                    this.setGrade(grade_);
                if(ccStandardCode_)
                    this.setCommonCoreStandardCode(ccStandardCode_);
            },

            String, function getUrlComponents(index_, needIsAllStandardCodes_) {
                //index_ = index_ != undefined ? index_|0 : 0;
                //var resArr = [
                //    'standardId[' + index_ + ']=' + encodeURIComponent(this.getAcademicBenchmarkId() || ''),
                //    'ccStandardCode[' + index_ + ']=' + encodeURIComponent(this.getCommonCoreStandardCode() || ''),
                //    'standardName[' + index_ + ']=' + encodeURIComponent(this.getName() || '')
                //]
                //
                //if(needIsAllStandardCodes_)
                //    resArr.push('isAllStandardCodes=' + (!this.commonCoreStandardCode || this.commonCoreStandardCode.length < 2))
                //
                //return resArr.join('&');

                /*index_ = index_ != undefined ? index_|0 : 0;
                var codes = this.getCommonCoreStandardCode().split(',').map(function(item, i, array){
                    return 'ccStandardCode[' + i + ']=' + encodeURIComponent(array[i] || '')
                });
                return codes.concat([
                    'standardId[' + index_ + ']=' + encodeURIComponent(this.getAcademicBenchmarkId() || ''),
                    'standardName[' + index_ + ']=' + encodeURIComponent(this.getName() || '')
                ]).join('&');*/
                return SELF.GET_URL_COMPONENTS(this, index_, needIsAllStandardCodes_)
            },

            String, function GET_URL_COMPONENTS(standard, index_, needIsAllStandardCodes_){
                index_ = index_ != undefined ? index_|0 : 0;
                var resArr = [
                    'standardId[' + index_ + ']=' + encodeURIComponent(standard.getAcademicBenchmarkId() || ''),
                    'ccStandardCode[' + index_ + ']=' + encodeURIComponent(standard.getCommonCoreStandardCode() || ''),
                    'standardName[' + index_ + ']=' + encodeURIComponent(standard.getName() || '')
                ];

                if(needIsAllStandardCodes_)
                    resArr.push('isAllStandardCodes=' + (!standard.getCommonCoreStandardCode() || standard.getCommonCoreStandardCode().length < 2))

                return resArr.join('&');
            },

            String, function GET_URL_COMPONENTS_FROM_STANDARDS(standards){
                var standardsWithMoreCodes = (standards || []).filter(function(item){
                    var codes = item.getCommonCoreStandardCodesArray();
                    return codes && codes.length > 1;
                });
                var isAllStandardCodes = standardsWithMoreCodes.length > 0;
                return (standards || []).map(function (c, index) { return c.getUrlComponents(index); }).join('&')
                    + '&isAllStandardCodes=' + isAllStandardCodes;
            },


            Object, function BUILD_URL_PARAMS_FROM_STANDARD(outParamsObj, standard, index_){
                outParamsObj = outParamsObj || {};
                outParamsObj['standardId[' + index_ + ']'] = standard.getAcademicBenchmarkId() || '';
                outParamsObj['ccStandardCode[' + index_ + ']'] = standard.getCommonCoreStandardCode() || '';
                outParamsObj['standardName[' + index_ + ']'] = standard.getName() || '';
                return outParamsObj;
            },

            Object, function BUILD_URL_PARAMS_FROM_STANDARDS(standards){
                var standardsWithMoreCodes = (standards || []).filter(function(item){
                    var codes = item.getCommonCoreStandardCodesArray();
                    return codes && codes.length > 1;
                });
                var isAllStandardCodes = standardsWithMoreCodes.length > 0;

                var res = {};
                (standards || []).forEach(function(s, index){
                    res = chlk.models.standard.Standard.BUILD_URL_PARAMS_FROM_STANDARD(res, s, index);
                });
                res.isAllStandardCodes = isAllStandardCodes;
                return res;
            }
        ]);
});
