REQUIRE('chlk.models.grading.GradingClassSummaryGridItems');
REQUIRE('chlk.models.common.PageWithClasses');
REQUIRE('chlk.models.announcement.ShortAnnouncementViewData');

NAMESPACE('chlk.models.grading', function () {
    "use strict";
    /** @class chlk.models.grading.GradingClassSummaryGridViewData*/
    CLASS(
        GENERIC('TItem'),
        'GradingClassSummaryGridViewData', EXTENDS(chlk.models.common.PageWithClasses), [
            ArrayOf(chlk.models.grading.GradingClassSummaryGridItems.OF(TItem)), 'items',

            ArrayOf(chlk.models.grading.AlphaGrade), 'alphaGrades',

            ArrayOf(chlk.models.grading.AlternateScore), 'alternateScores',

            [[chlk.models.classes.ClassesForTopBar, chlk.models.id.ClassId, ArrayOf(chlk.models.grading.GradingClassSummaryGridItems.OF(TItem)),
                ArrayOf(chlk.models.grading.AlphaGrade), ArrayOf(chlk.models.grading.AlternateScore)]],
            function $(topData_, selectedId_, items_, alphaGrades_, alternateScores_){
                BASE(topData_, selectedId_);
                if(items_)
                    this.setItems(items_);
                if(alphaGrades_)
                    this.setAlphaGrades(alphaGrades_);
                if(alternateScores_)
                    this.setAlternateScores(alternateScores_);
            }
        ]);
});