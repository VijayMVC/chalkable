REQUIRE('ria.serialize.IDeserializable');

NAMESPACE('chlk.models.bgtasks', function () {
    "use strict";
    /** @class chlk.models.bgtasks.BgTaskTypeEnum*/
    ENUM(
        'BgTaskTypeEnum', {
            CREATE_EMPTY_SCHOOL: 0,
            SIS_DATA_IMPORT:1,
            BACKUP_DATABASES: 2,
            RESTORE_DATABASES: 3,
            DATABASE_UPDATE: 4,
            CREATE_DEMO_SCHOOL: 5,
            DELETE_SCHOOL: 6
        });


    /** @class chlk.models.bgtasks.BgTaskType*/
    CLASS(
        'BgTaskType', IMPLEMENTS(ria.serialize.IDeserializable),  [
            chlk.models.bgtasks.BgTaskTypeEnum, 'typeId',
            function $(){
              this._types = {};
              this._types[chlk.models.bgtasks.BgTaskTypeEnum.CREATE_EMPTY_SCHOOL] = "Create empty school";
              this._types[chlk.models.bgtasks.BgTaskTypeEnum.SIS_DATA_IMPORT] = "SIS data import";
              this._types[chlk.models.bgtasks.BgTaskTypeEnum.BACKUP_DATABASES] = "Backup datatabases";
              this._types[chlk.models.bgtasks.BgTaskTypeEnum.RESTORE_DATABASES] = "Restore databases";
              this._types[chlk.models.bgtasks.BgTaskTypeEnum.DATABASE_UPDATE] = "Database Update";
              this._types[chlk.models.bgtasks.BgTaskTypeEnum.CREATE_DEMO_SCHOOL] = "Create Demo School";
              this._types[chlk.models.bgtasks.BgTaskTypeEnum.DELETE_SCHOOL] = "Delete School";
            },
            String, function toString(){
                return this._types[this.getTypeId()];
            },
            VOID, function deserialize(raw) {
                this.setTypeId(new chlk.models.bgtasks.BgTaskTypeEnum(Number(raw)));
            }
        ]);

});
