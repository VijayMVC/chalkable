REQUIRE('chlk.models.grading.GradingClassSummaryGridItems');
REQUIRE('chlk.models.common.PageWithClassesAndGradingPeriodsViewData');
REQUIRE('chlk.models.announcement.ShortAnnouncementViewData');
REQUIRE('chlk.models.id.GradingPeriodId');

NAMESPACE('chlk.models.grading', function () {
    "use strict";
    /** @class chlk.models.grading.GradingClassSummaryGridViewData*/
    CLASS(
        GENERIC('TItem'),
        'GradingClassSummaryGridViewData', EXTENDS(chlk.models.common.PageWithClassesAndGradingPeriodsViewData), [
            ArrayOf(chlk.models.grading.GradingClassSummaryGridItems.OF(TItem)), 'items',

            ArrayOf(chlk.models.grading.AlphaGrade), 'alphaGrades',

            ArrayOf(chlk.models.grading.AlternateScore), 'alternateScores',

            Boolean, 'ableEdit',

            [[chlk.models.id.GradingPeriodId, chlk.models.classes.ClassesForTopBar, chlk.models.id.ClassId, ArrayOf(chlk.models.grading.GradingClassSummaryGridItems.OF(TItem)),
                ArrayOf(chlk.models.grading.AlphaGrade), ArrayOf(chlk.models.grading.AlternateScore), Boolean]],
            function $(gradingPeriodId_, topData_, selectedId_, items_, alphaGrades_, alternateScores_, ableEdit_){
                BASE(gradingPeriodId_, topData_, selectedId_);
                if(items_)
                    this.setItems(items_);
                if(ableEdit_ || ableEdit_ === false)
                    this.setAbleEdit(ableEdit_);
                if(alphaGrades_)
                    this.setAlphaGrades(alphaGrades_);
                if(alternateScores_)
                    this.setAlternateScores(alternateScores_);
            }
        ]);
});