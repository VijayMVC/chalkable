REQUIRE('ria.serialize.SJX');
REQUIRE('chlk.models.profile.StandardizedTestViewData');

NAMESPACE('chlk.models.controls', function () {
    "use strict";

    /** @class chlk.models.controls.PanoramaFilterBlockViewData*/
    CLASS(
        'PanoramaFilterBlockViewData', [
            ArrayOf(chlk.models.profile.StandardizedTestViewData), 'standardizedTests',

            chlk.models.id.StandardizedTestId, 'standardizedTestId',

            chlk.models.id.StandardizedTestItemId, 'scoreTypeId',

            chlk.models.id.StandardizedTestItemId, 'componentId',

            [[ArrayOf(chlk.models.profile.StandardizedTestViewData, chlk.models.id.StandardizedTestId, chlk.models.id.StandardizedTestItemId, chlk.models.id.StandardizedTestItemId)]],
            function $(standardizedTests, standardizedTestId_, componentId_, scoreTypeId_){
                BASE();
                this.setStandardizedTests(standardizedTests);
                standardizedTestId_ && this.setStandardizedTestId(standardizedTestId_);
                componentId_ && this.setComponentId(componentId_);
                scoreTypeId_ && this.setScoreTypeId(scoreTypeId_);
            }
        ]);
});
