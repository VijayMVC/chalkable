NAMESPACE('chlk.models.apps', function(){
    /** @class chlk.models.apps.AppActionTypes*/
    ENUM('AppActionTypes', {
        ADD_ME: 'addMe',
        CLOSE_ME : 'closeMe',
        SAVE_ME: 'saveMe',
        SHOW_PLUS: 'showPlus',
        APP_ERROR: 'appError',
        ALERT_BOX: 'showAlertBox',
        PROMPT_BOX: 'showPromptBox',
        CONFIRM_BOX: 'showConfirmBox',
        STANDARD_PICKER: 'showStandardPicker',
        TOPIC_PICKER: 'showTopicsPicker'
    });
})



