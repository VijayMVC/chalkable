REQUIRE('chlk.models.apps.Application');
REQUIRE('chlk.models.apps.AppCategory');
REQUIRE('chlk.models.common.NameId');

NAMESPACE('chlk.models.apps', function () {
    "use strict";
    /** @class chlk.models.apps.AppInfoViewData*/
    CLASS(
        'AppInfoViewData', [
            chlk.models.apps.Application, 'app',
            ArrayOf(chlk.models.apps.AppCategory), 'categories',
            ArrayOf(chlk.models.common.NameId), 'gradeLevels',
            Boolean, 'empty',
            Boolean, 'readOnly',

            [[chlk.models.apps.Application, Boolean, ArrayOf(chlk.models.apps.AppCategory), ArrayOf(chlk.models.common.NameId)]],
            function $(app_, isReadonly, categories, gradeLevels){
                if (app_)
                    this.setApp(app_);
                this.setEmpty(!!app_);
                this.setReadOnly(isReadonly);
                this.setCategories(categories);
                this.setGradeLevels(gradeLevels);
            }
        ]);
});
