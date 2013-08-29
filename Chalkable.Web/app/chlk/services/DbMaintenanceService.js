REQUIRE('chlk.services.BaseService');
REQUIRE('ria.async.Future');
REQUIRE('chlk.models.departments.Department');
REQUIRE('chlk.models.dbmaintenance.DbBackup');
REQUIRE('chlk.models.Success');



NAMESPACE('chlk.services', function () {
    "use strict";
    /** @class chlk.services.DbMaintenanceService*/
    CLASS(
        'DbMaintenanceService', EXTENDS(chlk.services.BaseService), [
            ria.async.Future, function backup() {
                return this.get('dbmaintenance/Backup.json', chlk.models.Success, {});
            },

            [[String]],
            ria.async.Future, function restore(ticks) {
                return this.get('dbmaintenance/Restore.json', chlk.models.Success, {
                    time: ticks
                });
            },

            [[String, String]],
            ria.async.Future, function databaseUpdate(masterSql, schoolSql) {
                return this.post('dbmaintenance/DatabaseUpdate.json', chlk.models.Success, {
                    masterSql: masterSql,
                    schoolSql: schoolSql
                });
            },

            [[Number, Number]],
            ria.async.Future, function getBackups(start_, count_) {
                return this.getPaginatedList('dbmaintenance/ListBackups.json', chlk.models.dbmaintenance.DbBackup, {
                    start: start_,
                    count: count_
                });
            }
        ])
});