REQUIRE('chlk.models.apps.Application');
REQUIRE('chlk.models.apps.AppCategory');
REQUIRE('chlk.models.apps.AppPermission');
REQUIRE('chlk.models.apps.AppGradeLevel');
REQUIRE('chlk.models.apps.AppPlatform');


NAMESPACE('chlk.models.apps', function () {
    "use strict";
    /** @class chlk.models.apps.AppInfoViewData*/
    CLASS(
        'AppInfoViewData', [
            chlk.models.apps.Application, 'app',
            ArrayOf(chlk.models.apps.AppCategory), 'categories',
            ArrayOf(chlk.models.apps.AppGradeLevel), 'gradeLevels',
            ArrayOf(chlk.models.apps.AppPermission), 'permissions',
            ArrayOf(chlk.models.apps.AppPlatform), 'platforms',
            ArrayOf(chlk.models.apps.AppPlatform), 'supportedPlatforms',
            Boolean, 'empty',
            Boolean, 'readOnly',
            Boolean, 'draft',

            [[
                chlk.models.apps.Application, Boolean,
                ArrayOf(chlk.models.apps.AppCategory),
                ArrayOf(chlk.models.apps.AppGradeLevel),
                ArrayOf(chlk.models.apps.AppPermission),
                ArrayOf(chlk.models.apps.AppPlatform),
                Boolean
            ]],
            function $(app_, isReadonly, categories, gradeLevels, permissions, platforms, isDraft){
                BASE();
                if (app_)
                    this.setApp(app_);
                this.setEmpty(!!app_);
                this.setReadOnly(isReadonly);
                this.setCategories(categories);
                this.setGradeLevels(gradeLevels);
                this.setPermissions(permissions);
                this.setSupportedPlatforms(platforms);
                this.setDraft(isDraft);
            }
        ]);
});
