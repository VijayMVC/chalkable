NAMESPACE('chlk.models.settings', function () {
    "use strict";
    /** @class chlk.models.settings.Dashboard*/
    CLASS(
        'Dashboard', [
            Boolean, 'departmentsVisible',
            Boolean, 'storageMonitorVisible',
            Boolean, 'preferencesVisible',
            Boolean, 'appCategoriesVisible',
            Boolean, 'backgroundTaskMonitorVisible',

        ]);
});
