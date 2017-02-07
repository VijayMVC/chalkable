REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.settings.Preference');
REQUIRE('chlk.models.settings.PreferencesList');
REQUIRE('chlk.models.common.ActionLinkModel');

NAMESPACE('chlk.templates.settings', function () {

    ASSET('~/assets/jade/activities/profile/ProfileTopBar.jade')();
    /** @class chlk.templates.settings.Preferences*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/settings/Preferences.jade')],
        [ria.templates.ModelBind(chlk.models.settings.PreferencesList)],
        'Preferences', EXTENDS(chlk.templates.ChlkTemplate), [
            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.settings.Preference), 'items',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.settings.PreferenceCategory), 'preferenceCategories',

            [ria.templates.ModelPropertyBind],
            chlk.models.settings.PreferenceCategory, 'currentCategory',

            ArrayOf(chlk.models.common.ActionLinkModel), function buildActionLinkModels(pressedAction){
                var categories = this.getPreferenceCategories();
                return categories.map(function(c){
                        return this.createActionLinkModel_(c);
                    }, this);
            },

            chlk.models.common.ActionLinkModel, function createActionLinkModel_(category){
                var args = [category.getCategoryId()];
                var currentCategory = this.getCurrentCategory();
                return new chlk.models.common.ActionLinkModel('settings', 'preferences', category.toString()
                    ,  currentCategory.getCategoryId() == category.getCategoryId(), args, null, false);
            }
        ])
});