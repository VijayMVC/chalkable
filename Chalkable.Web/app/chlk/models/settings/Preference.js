NAMESPACE('chlk.models.settings', function () {
    "use strict";

    /** @class chlk.models.settings.PreferenceEnum*/
    ENUM(
        'PreferenceEnum', {
            VIDEO_GETTING_INFO_CHALKABLE: 'videogetinginfoschalkable'
        });

    /** @class chlk.models.settings.PreferenceCategoryEnum*/
    ENUM('PreferenceCategoryEnum',{
        COMMON: 0,
        EMAIL_TEXT: 1,
        CONTROLLER_DESCRIPTIONS: 2
    });

    /** @class chlk.models.settings.PreferenceCategory*/
    CLASS(
        'PreferenceCategory', IMPLEMENTS(ria.serialize.IDeserializable),  [
            chlk.models.settings.PreferenceCategoryEnum, 'categoryId',

            [[chlk.models.settings.PreferenceCategoryEnum]],
            function $(categoryId_){
                BASE();
                this._categories = {};
                this._categories[chlk.models.settings.PreferenceCategoryEnum.COMMON] = "Common";
                this._categories[chlk.models.settings.PreferenceCategoryEnum.EMAIL_TEXT] = "Email text";
                this._categories[chlk.models.settings.PreferenceCategoryEnum.CONTROLLER_DESCRIPTIONS] = "Controller Description";
                if(categoryId_)
                    this.setCategoryId(categoryId_)
            },
            String, function toString(){
                return this._categories[this.getCategoryId()]  || ('Unknown value: ' + this.getCategoryId().toString());
            },
            VOID, function deserialize(raw) {
                this.setCategoryId(chlk.models.settings.PreferenceCategoryEnum(Number(raw)));
            }
        ]);

    /** @class chlk.models.settings.Preference*/
    CLASS(
        'Preference', [
            String, 'key',
            String, 'value',
            [ria.serialize.SerializeProperty('ispublicpref')],
            Boolean, 'publicPreference',
            chlk.models.settings.PreferenceCategoryEnum, 'category',
            Number, 'type',
            String, 'hint'
        ]);
});
