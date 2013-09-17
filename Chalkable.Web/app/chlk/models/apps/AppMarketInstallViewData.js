REQUIRE('chlk.models.apps.AppMarketApplication');
REQUIRE('chlk.models.apps.AppInstallGroup');

NAMESPACE('chlk.models.apps', function () {
    "use strict";
    /** @class chlk.models.apps.AppMarketInstallViewData*/
    CLASS(
        'AppMarketInstallViewData', [

            chlk.models.apps.AppMarketApplication, 'app',
            String, 'installGroupColumnWidth',

            ArrayOf(ArrayOf(chlk.models.apps.AppInstallGroup)), 'installGroupColumns',

            [[ArrayOf(chlk.models.apps.AppInstallGroup)]],
            function getSeparatedInstallGroups(installGroup){
                var len = installGroup.length;
                if(len < 6)  return installGroup;
                if(len < 11) return [
                    installGroup.slice(0, 5),
                    installGroup.slice(5)
                ];

                if(len < 16){
                    return [
                        installGroup.slice(0, 5),
                        installGroup.slice(5, 10),
                        installGroup.slice(10)
                    ];
                }else {
                    var maxRows = Math.ceil(len / 3);
                    return [
                        installGroup.slice(0, maxRows),
                        installGroup.slice(maxRows, maxRows * 2),
                        installGroup.slice(maxRows * 2)
                    ];
                }
            },

            [[chlk.models.apps.AppMarketApplication]],
            function $(app){
                BASE();
                this.setApp(app);
                var installedForGroups = app.getInstalledForGroups() || [];
                var installGroupColumns = this.getSeparatedInstallGroups(installedForGroups);

                this.setInstallGroupColumns(installGroupColumns);
                this.setInstallGroupColumnWidth(parseInt(100 / (installGroupColumns.length || 1), 10) + '%');


            }
        ]);


});
