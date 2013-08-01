REQUIRE('chlk.services.BaseService');
REQUIRE('ria.async.Future');
REQUIRE('chlk.models.departments.Department');

NAMESPACE('chlk.services', function () {
    "use strict";
    /** @class chlk.services.DbMaintenanceService*/
    CLASS(
        'DbMaintenanceService', EXTENDS(chlk.services.BaseService), [
            ria.async.Future, function backup() {
                return this.get('DbMaintenance/Backup.json', chlk.models.Success, {});
            },

            [[Number]],
            ria.async.Future, function restore(size) {
                return this.get('DbMaintenance/Restore.json', chlk.models.Success, {
                    time: size
                });
            },

            [[String, String]],
            ria.async.Future, function databaseUpdate(masterSql, schoolSql) {
                return this.get('DbMaintenance/DatabaseUpdate.json', chlk.models.Success, {
                    masterSql: masterSql,
                    schoolSql: schoolSql
                });
            },

            [[Number, Number]],
            ria.async.Future, function getBackups(start_, count_) {
                return this.getPaginatedList('DbMaintenance/ListBackups.json', chlk.models.storage.Blob, {
                    start: start_,
                    count: count_
                });
            }
        ])
});