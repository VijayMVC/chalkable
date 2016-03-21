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
                this.deepest = SJX.fromValue(raw.isdeepest, Boolean);
                this.tooltip = SJX.fromValue(raw.tooltip, String);
                this.description = SJX.fromValue(raw.description, String);
                this.announcementId = SJX.fromValue(raw.announcementid, chlk.models.id.AnnouncementId);
                this.standardId = SJX.fromValue(raw.standardid, chlk.models.id.StandardId);
                this.grade = SJX.fromValue(raw.grade, String);
                this.academicBenchmarkId = SJX.fromValue(raw.academicbenchmarkid, String);
                this.subjectId = SJX.fromValue(raw.standardsubjectid, chlk.models.id.StandardSubjectId);
            },

            String, 'academicBenchmarkId',
            String, 'name',
            String, 'tooltip',
            String, 'description',
            Boolean, 'deepest',
            chlk.models.id.AnnouncementId, 'announcementId',
            chlk.models.id.StandardId, 'standardId',
            String, 'grade',
            chlk.models.id.StandardSubjectId, 'subjectId',

            function getId(){
                return this.getStandardId();
            },

            String, function displayTitle(){
                var name = this.getName();
                if(name && (!name.trim || name.trim() != '')) return name;
                return this.getDescription();
            },

            [[chlk.models.id.StandardId, String, String]],
            function $(standardId_, name_, grade_){
                BASE();
                if(standardId_)
                    this.setStandardId(standardId_);
                if(name_)
                    this.setName(name_);
                if(grade_)
                    this.setGrade(grade_);
            },

            String, function getUrlComponents(index_) {
                return SELF.GET_URL_COMPONENTS(this, index_)
            },

            String, function GET_URL_COMPONENTS(standard, index_){
                index_ = index_ != undefined ? index_|0 : 0;
                var resArr = [
                    'standardId[' + index_ + ']=' + encodeURIComponent(standard.getAcademicBenchmarkId() || ''),
                    'standardName[' + index_ + ']=' + encodeURIComponent(standard.getName() || '')
                ];

                return resArr.join('&');
            },

            String, function GET_URL_COMPONENTS_FROM_STANDARDS(standards){
                return (standards || []).map(function (c, index) { return c.getUrlComponents(index); }).join('&')
            },


            Object, function BUILD_URL_PARAMS_FROM_STANDARD(outParamsObj, standard, index_){
                outParamsObj = outParamsObj || {};
                outParamsObj['standardId[' + index_ + ']'] = standard.getAcademicBenchmarkId() || '';
                outParamsObj['standardName[' + index_ + ']'] = standard.getName() || '';
                return outParamsObj;
            },

            Object, function BUILD_URL_PARAMS_FROM_STANDARDS(standards){
                var res = {};
                (standards || []).forEach(function(s, index){
                    res = chlk.models.standard.Standard.BUILD_URL_PARAMS_FROM_STANDARD(res, s, index);
                });
                return res;
            }
        ]);
});
