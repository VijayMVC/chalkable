NAMESPACE('chlk.models.apps', function(){
    /** @class chlk.models.apps.AppModes*/
    ENUM('AppModes', {
        EDIT: 'editurl',
        VIEW: 'viewurl',
        MYAPPSVIEW: 'myappsurl',
        GRADINGVIEW: 'gradingviewurl',
        ATTACH: 'attach',
        SYSADMIN_VIEW: 'sysadminview',
        MY_VIEW: 'myview',
        SETTINGS_VIEW: 'settingsview',
        CONTENT_QUERY: 'content-query',
    });
})



