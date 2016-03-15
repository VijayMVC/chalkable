REQUIRE('chlk.services.BaseService');
REQUIRE('ria.async.Future');

REQUIRE('chlk.models.academicBenchmark.Authority');
REQUIRE('chlk.models.academicBenchmark.Document');
REQUIRE('chlk.models.academicBenchmark.SubjectDocument');
REQUIRE('chlk.models.academicBenchmark.GradeLevel');
REQUIRE('chlk.models.academicBenchmark.Standard');



NAMESPACE('chlk.services', function () {
    "use strict";

    /** @class chlk.services.CommonStandardService*/
    CLASS(
        'ABStandardService', EXTENDS(chlk.services.BaseService), [

            ria.async.Future, function getStandards(){

            },

            ria.async.Future, function getAuthority(){
                return this.get('AcademicBenchmark/Authorities.json', ArrayOf(chlk.models.academicBenchmark.Authority), {});
            },

            [[chlk.models.id.ABAuthorityId]],
            ria.async.Future, function getDocument(authorityId_){
                return this.get('AcademicBenchmark/Documents.json', ArrayOf(chlk.models.academicBenchmark.Document), {
                    authorityId: authorityId_ && authorityId_.valueOf(),
                });
            },

            [[chlk.models.id.ABAuthorityId, chlk.models.id.ABDocumentId]],
            ria.async.Future, function getSubjectDocuments(authorityId_, documentId_){
                return this.get('AcademicBenchmark/SubjectDocuments.json', ArrayOf(chlk.models.academicBenchmark.SubjectDocument), {
                    authorityId: authorityId_ && authorityId_.valueOf(),
                    documentId: documentId_ && documentId_.valueOf(),
                });
            },

            [[chlk.models.id.ABAuthorityId, chlk.models.id.ABDocumentId, chlk.models.id.ABSubjectDocumentId]],
            ria.async.Future, function getGradeLevels(authorityId_, documentId_, subjectDocId_){
                return this.get('AcademicBenchmark/SubjectDocuments.json', ArrayOf(chlk.models.academicBenchmark.GradeLevel), {
                    authorityId: authorityId_ && authorityId_.valueOf(),
                    documentId: documentId_ && documentId_.valueOf(),
                    subjectDocId: subjectDocId_ && subjectDocId_.valueOf(),
                });
            },


            [[chlk.models.id.ABAuthorityId, chlk.models.id.ABDocumentId, chlk.models.id.ABSubjectDocumentId, String, chlk.models.id.ABStandardId, Boolean]],
            ria.async.Future, function getStandards(authorityId_, documentId_, subjectDocId_, gradeLevelCode_, parentId_, firstLevelOnly_) {
                return this.get('AcademicBenchmark/Standards.json', ArrayOf(chlk.models.academicBenchmark.Standard), {
                    authorityId: authorityId_ && authorityId_.valueOf(),
                    documentId: documentId_ && documentId_.valueOf(),
                    subjectDocId: subjectDocId_ && subjectDocId_.valueOf(),
                    gradeLevelCode: gradeLevelCode_,
                    parentId: parentId_ && parentId_.valueOf(),
                    firstLevelOnly: firstLevelOnly_
                });
            },

            [[String, Number, Number]],
            ria.async.Future, function searchStandards(searchQuery, start_, count_){
                return this.getPaginatedList('AcademicBenchmark/SearchStandards.json', ArrayOf(chlk.models.academicBenchmark.Standard), {
                    searchQuery: searchQuery,
                    start: start_,
                    count: count_
                });
            },

            [[String, String, chlk.models.id.AbParentId, Boolean]],
            ria.async.Future, function getTopics(subjectCode, gradeLevel, parentId_, firstLevelOnly_){
                return this.getPaginatedList('AcademicBenchmark/Topics.json', ArrayOf(chlk.models.academicBenchmark.Topic),
                    {
                        subjectCode: subjectCode,
                        gradeLevel: gradeLevel,
                        parentId: parentId_ && parentId_.valueOf(),
                        firstLevelOnly: firstLevelOnly_ && firstLevelOnly_.valueOf()
                    });
            }
        ]);
});